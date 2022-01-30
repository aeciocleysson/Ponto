using System;

namespace MegaPonto.ViewModel
{
    public class PontoViewModel : BaseViewModel
    {
        public TimeSpan Entrada { get;  set; }
        public TimeSpan SaidaIntervalo { get;  set; }
        public TimeSpan RetornoIntervalo { get;  set; }
        public TimeSpan TotalIntervalo { get;  set; }
        public TimeSpan Saida { get;  set; }
        public TimeSpan TotalTrabalhado { get;  set; }
        public int FuncionarioId { get; set; }
        public long Matricula { get; set; }
        public double Minutos { get; set; }
    }
}
