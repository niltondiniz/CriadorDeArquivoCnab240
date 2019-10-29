using System;
using System.IO;
using System.Linq;

namespace GeradorDeArquivoCnab240.Console.Helpers
{
    public static class GeradorHelper
    {
        public static string ObterNomeArquivo(string diretorio, string param)
        {
            return
                $"{param.PadLeft(9, '0')}_{DateTime.Today:ddMMyyyy}_{ObterSequenciaArquivo(diretorio, param):000}.REM";
        }
        
        private static int ObterSequenciaArquivo(string diretorio, string param)
        {
            var sequencia = 1;

            if (VerificarDiretorioExistente(diretorio))
            {
                var listaArquivos = Directory.GetFiles(diretorio).ToList();

                if (listaArquivos.Count > 0)
                {
                    if (listaArquivos.Any(x => x.Contains($"{param}_{DateTime.Today:ddMMyyyy}")))
                    {
                        sequencia = listaArquivos.Count(x =>
                                        x.Contains($"{param}_{DateTime.Today:ddMMyyyy}")) + 1;
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(diretorio);
            }

            return sequencia;
        }
        
        private static bool VerificarDiretorioExistente(string diretorio)
        {
            var dir = new DirectoryInfo(diretorio);
            return dir.Exists;
        }
    }
}