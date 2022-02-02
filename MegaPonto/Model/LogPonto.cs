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
            FinalizouTrabalho = 4,

            [Description("Ponto Inserido manualmente")]
            PontoManual = 5,

            [Description("Trabalhado")]
            Trabalhado = 6,

            [Description("Atestado médico")]
            Atestado = 7,

            [Description("Faltou")]
            Faltou = 8
        }

        public int Log { get; private set; }
        public int FuncionarioId { get; private set; }
        public string Descricao { get; private set; }
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