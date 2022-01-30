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

            txtMatricula.Focus();
            txtMatricula.Select();
        }
        private void timerHora_Tick(object sender, EventArgs e)
        {
            this.lblHoraAtual.Text = DateTime.Now.ToString("HH:mm:ss");
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

                    var logModel = new LogPonto(log: logViewModel.Log, descricao: logViewModel.Descricao ,logViewModel.FuncionarioId);

                    _context.Add(model);
                    _context.SaveChanges();

                    _context.Add(logModel);
                    _context.SaveChanges();

                    GetAll();
                    return;
                }

                var logPonto = _context.Log.Where(w => w.FuncionarioId == funcionario.Id && w.Inserted == DateTime.Today).Max(w => w.Log);

                var scoreOutLanch = _context.Ponto.Where(a => a.FuncionarioId == funcionario.Id && a.Matricula == funcionario.Matricula && a.Inserted == date 
                                                              && logPonto == 1).SingleOrDefault();

                //    if (scoreOutLanch != null)
                //    {
                //        viewModel.OutLanch = TimeSpan.Parse(lblHoraAtual.Text);
                //        viewModel.Id = scoreOutLanch.Id;

                //        var result = _context.Scores.Find(viewModel.Id);

                //        result.UpdateOutLanch(outLanch: viewModel.OutLanch);

                //        logViewModel.Log = (int)StatusLog.ELog.SaidaAlmoco;
                //        logViewModel.EmployeeId = employee.Id;

                //        var logModel = new LogScore(log: logViewModel.Log, employeeId: logViewModel.EmployeeId);

                //        _context.Scores.Update(result);
                //        _context.SaveChanges();

                //        _context.Add(logModel);
                //        _context.SaveChanges();

                //        GetAll();
                //        ClearFields();
                //        return;
                //    }

                //    var scoreReturnLanch = _context.Scores.Where(a => a.EmployeeId == employee.Id && a.Code == employee.Code && a.Inserted == date
                //                                                 /*a.ReturnLunch != DateTime.Now*/ && logScore == 2).SingleOrDefault();

                //    if (scoreReturnLanch != null)
                //    {
                //        viewModel.ReturnLunch = TimeSpan.Parse(lblHoraAtual.Text);
                //        viewModel.Id = scoreReturnLanch.Id;
                //        viewModel.Code = scoreReturnLanch.Code;
                //        viewModel.FullRange = (viewModel.ReturnLunch - scoreReturnLanch.OutLanch);

                //        var result = _context.Scores.Find(viewModel.Id);

                //        result.UpdateReturnLanch(returnLanch: viewModel.ReturnLunch, fullRange: viewModel.FullRange);

                //        logViewModel.Log = (int)StatusLog.ELog.RetornoAlmoco;
                //        logViewModel.EmployeeId = employee.Id;

                //        var logModel = new LogScore(log: logViewModel.Log, employeeId: logViewModel.EmployeeId);

                //        _context.Scores.Update(result);
                //        _context.SaveChanges();

                //        _context.Add(logModel);
                //        _context.SaveChanges();

                //        GetAll();
                //        ClearFields();
                //        return;
                //    }

                //    var scoreDepartureTime = _context.Scores.Where(a => a.EmployeeId == employee.Id && a.Code == employee.Code && a.Inserted == date
                //                                                   /*a.DepartureTime != DateTime.Now*/ && logScore == 3).SingleOrDefault();

                //    if (scoreDepartureTime != null)
                //    {
                //        viewModel.DepartureTime = TimeSpan.Parse(lblHoraAtual.Text);
                //        viewModel.Worked = (viewModel.DepartureTime - scoreDepartureTime.EntryTime - scoreDepartureTime.FullRange);
                //        viewModel.Id = scoreDepartureTime.Id;

                //        viewModel.Minutes = viewModel.Worked.TotalMinutes;

                //        var result = _context.Scores.Find(viewModel.Id);

                //        result.UpdateDepartureTime(departureTime: viewModel.DepartureTime,
                //            worked: viewModel.Worked,
                //            minutes: viewModel.Minutes);

                //        logViewModel.Log = (int)StatusLog.ELog.FinalizouTrabalho;
                //        logViewModel.EmployeeId = employee.Id;

                //        var logModel = new LogScore(log: logViewModel.Log, employeeId: logViewModel.EmployeeId);

                //        _context.Scores.Update(result);
                //        _context.SaveChanges();

                //        _context.Add(logModel);
                //        _context.SaveChanges();

                //        GetAll();
                //        ClearFields();
                //    }
                //    else
                //    {
                //        MessageBox.Show($"{employee.Name}, você já marcou os pontos do dia!", "Alerta", MessageBoxButtons.OK,
                //        MessageBoxIcon.Information);
                //        ClearFields();
                //    }
            }
            else
            {
                MessageBox.Show("Funcionário não encontrado!", "Alerta", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                //ClearFields();
            }
        }

        private void txtMatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            long isNumber;

            if (e.KeyChar == 13)
            {
                //if (long.TryParse(txtMatricula.Text, out isNumber))
                //{
                InsertInput(Convert.ToInt64(txtMatricula.Text));
                //}
                //else
                //{
                //    MessageBox.Show("Esse campo só aceita número!", "Alerta", MessageBoxButtons.OK,
                //    MessageBoxIcon.Information);
                //}
            }
        }
    }
}
