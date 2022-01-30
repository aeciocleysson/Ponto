using System;
using System.ComponentModel;

namespace MegaPonto.Model
{
    public class LogPonto : BaseModel
    {
        public enum ELog : int
        {
            [Description("Iniciou os trabalhos")]
            InicioTrabalho = 1,

            [Description("Saiu para o almoço/intervalo")]
            SaidaAlmoco = 2,

            [Description("Retonou do almoço/intervalo")]
            RetornoAlmoco = 3,

            [Description("Finalizou os trabalhos")]
            FinalizouTrabalho = 4
        }

        public int Log { get; private set; }
        public int FuncionarioId { get; private set; }
        public string Descricao { get; private set; }
        //public ELog StatusLog { get; set; }
        public Funcionario Funcionario { get; set; }

        public LogPonto()
        {

        }
        public LogPonto(int log, string descricao, int funcionarioid)
        {
            Log = log;
            Descricao = descricao;
            FuncionarioId = funcionarioid;
        }
    }
}