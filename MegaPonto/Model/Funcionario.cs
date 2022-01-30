using System;
using System.Collections.Generic;
namespace MegaPonto.Model
{
    public class Funcionario : BaseModel
    {
        public string Nome { get; private set; }
        public long Matricula { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public virtual List<Ponto> Pontos { get; set; } = new List<Ponto>();
        public virtual List<LogPonto> LogPontos { get; set; } = new List<LogPonto>();

        public Funcionario(string nome, long matricula, DateTime dataNascimento)
        {
            Nome = nome;
            Matricula = matricula;
            DataNascimento = dataNascimento;
        }

        public Funcionario() { }

        public void Update(string nome, DateTime dataNascimento)
        {
            Nome = nome;
            DataNascimento = dataNascimento;
            UpdateAt = DateTime.Now;
        }

        public void Delete(bool isDelete)
        {
            IsDelete = isDelete;
            UpdateAt = DateTime.Now;
        }
    }
}
