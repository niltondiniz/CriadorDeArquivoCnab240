using System;
using GeradorDeArquivoCnab240.Console.Validadores;

namespace GeradorDeArquivoCnab240.Console.Entidade
{
    public class DadosHeaderArquivo
    {
        public string CnpjCliente { get; }
        public DateTime DataOperacao => DateTime.Now.Date;

        public bool IsValid
        {
            get => true;
            private set { }
        }

        public DadosHeaderArquivo(string cnpjCliente)
        {
            CnpjCliente = cnpjCliente;
            Validar();
        }

        private void Validar()
        {
            IsValid = CnpjValidador.Validar(CnpjCliente);
        }
    }
}