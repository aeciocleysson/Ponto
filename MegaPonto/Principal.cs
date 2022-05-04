using AntaresBackup;
using MegaPonto.Data;
using MegaPonto.Model;
using MegaPonto.ViewModel;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MegaPonto
{
    public partial class Principal : Form
    {
        private DataContext _context;
        private DateTime date = DateTime.Today;
        public TimeSpan horaBackup = TimeSpan.Parse("19:03:00");
        public Principal()
        {
            InitializeComponent();
            Starts();
        }

        private void Starts()
        {
            _context = new DataContext();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
            lblDateDay.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblDia.Text = DateTime.Now.ToString("dddd ,", new CultureInfo("pt-BR"));

            GetAll();
            ClearField();
        }

        private void timerHora_Tick(object sender, EventArgs e)
        {
            this.lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");

            // Rotina de backup diaria
            if (TimeSpan.Parse(lblHoraAtual.Text) == horaBackup && lblDia.Text != "domingo ,")
                CriarBackup.ExecutarBackup();
            else
                return;
        }

        private void ClearField()
        {
            txtMatricula.Clear();
            txtMatricula.Focus();
            txtMatricula.Select();
        }

        public void GetAll()
        {
            var funcionarioPonto = _context.Ponto
                .Where(w => w.Inserted == Convert.ToDateTime(lblDateDay.Text))
                .Select(s => new
                {
                    Código = s.Id,
                    Data = s.Inserted,
                    Funcionário = s.Funcionario.Nome,
                    Entrada = s.Entrada,
                    SaidaAlmoço = s.SaidaIntervalo,
                    RetornoAlmoço = s.RetornoIntervalo,
                    Intervalo = s.TotalIntervalo,
                    Saida = s.Saida,
                    Total = s.TotalTrabalhado,
                    Status = s.LogPontoId == 1 ? "Trabalhando" : s.LogPontoId == 2 ? "Intevalo" : s.LogPontoId == 3 ? "Trabalhando" : s.LogPontoId == 6 ? "Dia Finalizado" : ""
                }).OrderBy(o => o.Funcionário)
                .ToList();

            dgvScore.DataSource = funcionarioPonto;
            dgvScore.Columns["SaidaAlmoço"].HeaderText = "Saida Intervalo";
            dgvScore.Columns["RetornoAlmoço"].HeaderText = "Retorno Intervalo";
            dgvScore.Columns["Intervalo"].HeaderText = "Total Intervalo";
            dgvScore.Columns["Total"].DefaultCellStyle.Font = new Font(dgvScore.DefaultCellStyle.Font.FontFamily, 8, FontStyle.Bold);
            dgvScore.Columns["Status"].DefaultCellStyle.Font = new Font(dgvScore.DefaultCellStyle.Font.FontFamily, 8, FontStyle.Bold);
        }

        public void InsertInput(long matricula)
        {
            var funcionario = _context.Funcionario.SingleOrDefault(w => w.Matricula == matricula);

            if (!string.IsNullOrEmpty(txtMatricula.Text.Trim()) && funcionario != null)
            {
                var viewModel = new PontoViewModel();
                var logViewModel = new LogScoreViewModel();

                // Inicio das atividades
                var entrada = _context.Ponto.Any(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date);

                if (!entrada)
                {
                    viewModel.FuncionarioId = funcionario.Id;
                    viewModel.Matricula = funcionario.Matricula;
                    viewModel.Entrada = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.LogPontoId = (int)LogPonto.ELog.InicioTrabalho;

                    var model = new Ponto(entrada: viewModel.Entrada,
                        funcionarioId: viewModel.FuncionarioId,
                        matricula: viewModel.Matricula,
                        logPontoId: viewModel.LogPontoId);

                    logViewModel.StatusLogId = (int)LogPonto.ELog.InicioTrabalho;
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(statusLogId: logViewModel.StatusLogId, logViewModel.FuncionarioId);

                    _context.Add(model);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                // Saída para intervalo
                var logPonto = _context.Log.Where(w => w.FuncionarioId == funcionario.Id && w.Inserted == DateTime.Today).Max(w => w.StatusLogId);

                var saidaIntervalo = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                              && logPonto == (int)LogPonto.ELog.InicioTrabalho).SingleOrDefault();
                if (saidaIntervalo != null)
                {
                    viewModel.SaidaIntervalo = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.Id = saidaIntervalo.Id;
                    viewModel.LogPontoId = (int)LogPonto.ELog.SaidaAlmoco;

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.SaidaAlmocoIntervalo(saidaIntervalo: viewModel.SaidaIntervalo,
                                                logPontoId: viewModel.LogPontoId);

                    logViewModel.StatusLogId = (int)LogPonto.ELog.SaidaAlmoco;
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(statusLogId: logViewModel.StatusLogId, funcionarioid: logViewModel.FuncionarioId);

                    _context.Ponto.Update(result);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                // Retorno intervalo
                var retornoIntervalo = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                                  && logPonto == (int)LogPonto.ELog.SaidaAlmoco).SingleOrDefault();

                if (retornoIntervalo != null)
                {
                    viewModel.RetornoIntervalo = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.Id = retornoIntervalo.Id;
                    viewModel.TotalIntervalo = (viewModel.RetornoIntervalo - retornoIntervalo.SaidaIntervalo);
                    viewModel.LogPontoId = (int)LogPonto.ELog.RetornoAlmoco;

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.RetornoAlmocoIntervalo(retornoIntervalo: viewModel.RetornoIntervalo, totalIntervalo: viewModel.TotalIntervalo,
                                                   logPontoId: viewModel.LogPontoId);

                    logViewModel.StatusLogId = (int)LogPonto.ELog.RetornoAlmoco;
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(statusLogId: logViewModel.StatusLogId, funcionarioid: logViewModel.FuncionarioId);

                    _context.Ponto.Update(result);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                // Finalizando as atividades
                var saida = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                                    && logPonto == (int)LogPonto.ELog.RetornoAlmoco).SingleOrDefault();

                if (saida != null)
                {
                    viewModel.Saida = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.TotalTrabalhado = (viewModel.Saida - saida.Entrada - saida.TotalIntervalo);
                    viewModel.Id = saida.Id;
                    viewModel.LogPontoId = (int)LogPonto.ELog.Trabalhado;

                    viewModel.Minutos = viewModel.TotalTrabalhado.TotalMinutes;

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.FinalizarDia(saida: viewModel.Saida,
                        totalTrabalhado: viewModel.TotalTrabalhado,
                        minutos: viewModel.Minutos,
                        logPontoId: viewModel.LogPontoId);

                    logViewModel.StatusLogId = (int)LogPonto.ELog.Trabalhado;
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(statusLogId: logViewModel.StatusLogId, funcionarioid: logViewModel.FuncionarioId);

                    _context.Ponto.Update(result);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                }
                else
                {
                    MessageBox.Show($"{funcionario.Nome}, você já marcou os pontos do dia!", "Alerta", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                    ClearField();
                }
            }
            else
            {
                MessageBox.Show("Funcionário não encontrado!", "Alerta", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                ClearField();
            }
        }

        private void txtMatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            long isNumber;

            if (e.KeyChar == 13)
            {
                if (long.TryParse(txtMatricula.Text, out isNumber))
                {
                    InsertInput(Convert.ToInt64(txtMatricula.Text));
                }
                else
                {
                    MessageBox.Show("Esse campo só aceita número!", "Alerta", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                    ClearField();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timerGetAllPonto_Tick(object sender, EventArgs e)
        {
            Starts();
        }
    }
}
