using MegaPonto.Data;
using MegaPonto.Model;
using MegaPonto.ViewModel;
using System;
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
        public Principal()
        {
            InitializeComponent();
            Starts();
        }

        private void Starts()
        {
            _context = new DataContext();

            GetAll();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
            lblDateDay.Text = DateTime.Now.ToString("dd/MM/yyyy");

            ClearField();
        }

        private void timerHora_Tick(object sender, EventArgs e)
        {
            this.lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");
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
                .Where(w => w.Inserted == date)
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
                    Total = s.TotalTrabalhado
                }).OrderBy(o => o.Código)
                .ToList();

            dgvScore.DataSource = funcionarioPonto;
        }
              
        public void InsertInput(long matricula)
        {
            var funcionario = _context.Funcionario.SingleOrDefault(w => w.Matricula == matricula);

            if (!string.IsNullOrEmpty(txtMatricula.Text.Trim()) && funcionario != null)
            {
                var viewModel = new PontoViewModel();
                var logViewModel = new LogScoreViewModel();

                var entrada = _context.Ponto.Any(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date);

                if (!entrada)
                {
                    viewModel.FuncionarioId = funcionario.Id;
                    viewModel.Matricula = funcionario.Matricula;
                    viewModel.Entrada = TimeSpan.Parse(lblHoraAtual.Text);

                    var model = new Ponto(entrada: viewModel.Entrada,
                        funcionarioId: viewModel.FuncionarioId,
                        matricula: viewModel.Matricula);

                    logViewModel.Log = (int)LogPonto.ELog.InicioTrabalho;
                    logViewModel.Descricao = "Iniciou os trabalhos";
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(log: logViewModel.Log, descricao: logViewModel.Descricao, logViewModel.FuncionarioId);

                    _context.Add(model);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                var logPonto = _context.Log.Where(w => w.FuncionarioId == funcionario.Id && w.Inserted == DateTime.Today).Max(w => w.Log);

                var saidaIntervalo = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                              && logPonto == (int)LogPonto.ELog.InicioTrabalho).SingleOrDefault();

                if (saidaIntervalo != null)
                {
                    viewModel.SaidaIntervalo = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.Id = saidaIntervalo.Id;

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.SaidaAlmocoIntervalo(saidaIntervalo: viewModel.SaidaIntervalo);

                    logViewModel.Log = (int)LogPonto.ELog.SaidaAlmoco;
                    logViewModel.Descricao = "Saiu para o almoço/intervalo";
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(log: logViewModel.Log, descricao: logViewModel.Descricao, funcionarioid: logViewModel.FuncionarioId);

                    _context.Ponto.Update(result);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                var retornoIntervalo = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                                  && logPonto == (int)LogPonto.ELog.SaidaAlmoco).SingleOrDefault();

                if (retornoIntervalo != null)
                {
                    viewModel.RetornoIntervalo = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.Id = retornoIntervalo.Id;
                    viewModel.TotalIntervalo = (viewModel.RetornoIntervalo - retornoIntervalo.SaidaIntervalo);

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.RetornoAlmocoIntervalo(retornoIntervalo: viewModel.RetornoIntervalo, totalIntervalo: viewModel.TotalIntervalo);

                    logViewModel.Log = (int)LogPonto.ELog.RetornoAlmoco;
                    logViewModel.Descricao = "Retonou do almoço/intervalo";
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(log: logViewModel.Log, descricao: logViewModel.Descricao, funcionarioid: logViewModel.FuncionarioId);

                    _context.Ponto.Update(result);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    ClearField();
                    return;
                }

                var saida = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date
                                                                    && logPonto == (int)LogPonto.ELog.RetornoAlmoco).SingleOrDefault();

                if (saida != null)
                {
                    viewModel.Saida = TimeSpan.Parse(lblHoraAtual.Text);
                    viewModel.TotalTrabalhado = (viewModel.Saida - saida.Entrada - saida.TotalIntervalo);
                    viewModel.Id = saida.Id;

                    viewModel.Minutos = viewModel.TotalTrabalhado.TotalMinutes;

                    var result = _context.Ponto.Find(viewModel.Id);

                    result.FinalizarDia(saida: viewModel.Saida,
                        totalTrabalhado: viewModel.TotalTrabalhado,
                        minutos: viewModel.Minutos);

                    logViewModel.Log = (int)LogPonto.ELog.FinalizouTrabalho;
                    logViewModel.Descricao = "Finalizou os trabalhos";
                    logViewModel.FuncionarioId = funcionario.Id;

                    var logModel = new LogPonto(log: logViewModel.Log, descricao: logViewModel.Descricao, funcionarioid: logViewModel.FuncionarioId);

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
            GetAll();
            ClearField();
        }
    } 
}
