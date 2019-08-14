namespace GeradorDeArquivoCnab240.Console.Validadores
{
    public static class CnpjValidador
    {
        public static bool Validar(string cnpj)
        {
            var isValid = true;
            
            if (string.IsNullOrEmpty(cnpj)){
                isValid = false;
                return false;
            }
            if (cnpj.Length < 14)
                isValid = false;

            return isValid;
        }
    }
}