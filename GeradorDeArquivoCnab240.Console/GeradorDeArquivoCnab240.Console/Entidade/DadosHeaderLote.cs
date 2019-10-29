using GeradorDeArquivoCnab240.Console.Validadores;

namespace GeradorDeArquivoCnab240.Console.Entidade
{
    public class DadosHeaderLote
    {
        public bool IsValid
        {
            get => true;
            private set { }
        }

        public string CnpjCliente { get; private set; }


        public DadosHeaderLote(string cnpjCliente)
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