namespace WebApplication3.Validações
{
    public class ValidarCPF
    {
        public static bool Validar(string CPF)
        {
            bool cpfValido = true;


            //Verificar se o CPF tem 11 digitos
            if (CPF.Length != 11)
                cpfValido = false;
            else
            {
                //Verificar se todos os caracteres de CPF são digitos numéricos
                for (int i = 0; i < CPF.Length; i++)
                {
                    if (!char.IsDigit(CPF[i])) 
                    { 
                        cpfValido = false; 
                        break; 
                    }
                }
            }

            if (cpfValido)
            {
                for (byte i = 0; i < 10; i++)
                {
                    var temp = new string(Convert.ToChar(i), 11);
                    if (CPF == temp)
                    {
                        cpfValido = false;
                        break;
                    }
                }
            }

            //Verificar Digito de controle do CPF
            if (cpfValido)
            {
                var j = 0;
                var d1 = 0;
                var d2 = 0;

                // Validar o primeiro numero do digito de controle
                for (int i = 10; i > 1; i--)
                {
                    d1 += Convert.ToInt32(CPF.Substring(j, 1)) * i;
                    j++;

                }

                //resto da divisao
                d1 = (d1 * 10) % 10;
                if (d1 == 10)
                    d1 = 0;

                //Verificar se o primeiro número bateu na posição 9(penultima)
                if (d1 != Convert.ToInt32(CPF.Substring(j, 1)))
                    cpfValido = false;


                //Validar o segundo numero (digito) do controle
                if (cpfValido)
                {
                    j = 0;
                    for (int i = 11; i > 1; i--)
                    {
                        d2 += Convert.ToInt32(CPF.Substring(j, 1)) * i;
                        j++;
                    }

                    //resto da divisao
                    d2 = (d2 * 10) % 10;
                    if (d2 == 10)
                        d2 = 0;

                    //verificar se o segundo digito bateu na posição 10(ultima)
                    if (d2 != Convert.ToInt32(CPF.Substring(10, 1)))
                        cpfValido = false;
                }


            }


            return cpfValido;

        }
    }
}
