using System;

namespace MegaPonto.Model
{
    public class Ponto : BaseModel
    {
        public int FuncionarioId { get; private set; }
        public long Matricula { get; private set; }
        public TimeSpan Entrada { get; private set; }
        public TimeSpan SaidaIntervalo { get; private set; }
        public TimeSpan RetornoIntervalo { get; private set; }
        public TimeSpan TotalIntervalo { get; private set; }
        public TimeSpan Saida { get; private set; }
        public TimeSpan TotalTrabalhado { get; private set; }
        public double Minutos { get; private set; }
        public virtual Funcionario Funcionario { get; private set; }

        public Ponto() { }

        public Ponto(int funcionarioId, long matricula, TimeSpan entrada)
        {
            FuncionarioId = funcionarioId;
            Matricula = matricula;
            Entrada = entrada;
        }

        public void SaidaAlmocoIntervalo(TimeSpan saidaIntervalo)
        {
            SaidaIntervalo = saidaIntervalo;
        }

        public void RetornoAlmocoIntervalo(TimeSpan retornoIntervalo, TimeSpan totalIntervalo)
        {
            RetornoIntervalo = retornoIntervalo;
            TotalIntervalo = totalIntervalo;
        }

        public void FinalizarDia(TimeSpan saida, TimeSpan totalTrabalhado, double minutos)
        {
            Saida = saida;
            TotalTrabalhado = totalTrabalhado;
            Minutos = minutos;
            UpdateAt = DateTime.Now;
        }

        public void UpdateHours(TimeSpan entrada, TimeSpan saidaIntervalo, TimeSpan retornoIntervalo, TimeSpan totalIntervlo,
                                TimeSpan saida, TimeSpan totalTrabalhado, double minutos)
        {
            Entrada = entrada;
            SaidaIntervalo = saidaIntervalo;
            RetornoIntervalo = retornoIntervalo;
            TotalIntervalo = totalIntervlo;
            Saida = saida;
            TotalTrabalhado = totalTrabalhado;
            Minutos = minutos;
        }
    }
}
