
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Algorithm.Logic
{
    using System;

    public class Program
    {
        /// <summary>
        /// PROBLEMA:
        /// 
        /// Implementar um algoritmo para o controle de posição de um drone emum plano cartesiano (X, Y).
        /// 
        /// O ponto inicial do drone é "(0, 0)" para cada execução do método Evaluate ao ser executado cada teste unitário.
        /// 
        /// A string de entrada pode conter os seguintes caracteres N, S, L, e O representando Norte, Sul, Leste e Oeste respectivamente.
        /// Estes catacteres podem estar presentes aleatóriamente na string de entrada.
        /// Uma string de entrada "NNNLLL" irá resultar em uma posição final "(3, 3)", assim como uma string "NLNLNL" irá resultar em "(3, 3)".
        /// 
        /// Caso o caracter X esteja presente, o mesmo irá cancelar a operação anterior. 
        /// Caso houver mais de um caracter X consecutivo, o mesmo cancelará mais de uma ação na quantidade em que o X estiver presente.
        /// Uma string de entrada "NNNXLLLXX" irá resultar em uma posição final "(1, 2)" pois a string poderia ser simplificada para "NNL".
        /// 
        /// Além disso, um número pode estar presente após o caracter da operação, representando o "passo" que a operação deve acumular.
		/// Este número deve estar compreendido entre 1 e 2147483647.
		/// Deve-se observar que a operação 'X' não suporta opção de "passo" e deve ser considerado inválido. Uma string de entrada "NNX2" deve ser considerada inválida.
        /// Uma string de entrada "N123LSX" irá resultar em uma posição final "(1, 123)" pois a string pode ser simplificada para "N123L"
        /// Uma string de entrada "NLS3X" irá resultar em uma posição final "(1, 1)" pois a string pode ser siplificada para "NL".
        /// 
        /// Caso a string de entrada seja inválida ou tenha algum outro problema, o resultado deve ser "(999, 999)".
        /// 
        /// OBSERVAÇÕES:
        /// Realizar uma implementação com padrões de código para ambiente de "produção". 
        /// Comentar o código explicando o que for relevânte para a solução do problema.
        /// Adicionar testes unitários para alcançar uma cobertura de testes relevânte.
        /// </summary>
        /// <param name="input">String no padrão "N1N2S3S4L5L6O7O8X"</param>
        /// <returns>String representando o ponto cartesiano após a execução dos comandos (X, Y)</returns>
        public static string Evaluate(string input)
        {
            return DoOperation(input);
        }

        private static string DoOperation(string input)
        {
            if (IsValidString(input))
                return CartesianPlan
                    .WithPostion(999, 999)
                    .GetPosition();

            //Cancela os movimentos necessários
            input = CancelMoviements(input);

            //Verifica se os comandos de entrada não foram anulados
            if (AllCanceledMoviements(input))
                return CartesianPlan
                    .WithPostion(999, 999)
                    .GetPosition();


            //Regex para criar grupos para executar os passos
            Regex groupRegex = new Regex(@"(?<N>N+\d*X*\d*)|(?<S>S+\d*X*\d*)|(?<L>L+\d*X*\d*)|(?<O>O+\d*X*\d*)");

            var resultList = GetGroupsCommands(input, groupRegex);
            var cartesianPlan = new CartesianPlan();

            foreach (var value in resultList)
            {
                var res = value;
                if (int.TryParse(GetNumber(res, 1), out var repeteValue))
                {
                    //valida se a quantidade de repetições não gera overflow
                    if (IsOverflowValue(repeteValue))
                        return CartesianPlan
                        .WithPostion(999, 999)
                        .GetPosition();

                    cartesianPlan.MoveDirection(res, repeteValue);
                }
                else
                {
                    cartesianPlan.MoveDirection(res);
                }
            }

            return cartesianPlan.GetPosition();
        }

        private static bool AllCanceledMoviements(string input)
        {
            return string.IsNullOrEmpty(string.Join("", input));
        }

        /// <summary>
        /// Valida se a string é valida, 
        /// Este metodo não valida se a string cancela todos seus movimentos.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsValidString(string input)
        {
            Regex rx = new Regex("[^NSLOX0-9]");
            return string.IsNullOrEmpty(input)
                   || string.IsNullOrEmpty(input.Trim())
                   || int.TryParse(input[0].ToString().Trim(), out var v)
                   || rx.IsMatch(input);
        }

        /// <summary>
        /// Valida se o valor esta dentro dos limites.
        /// </summary>
        /// <param name="repeteValue"></param>
        /// <returns></returns>
        private static bool IsOverflowValue(int repeteValue)
        {
            return (repeteValue < 0 || repeteValue >= 2147483647);
        }

        /// <summary>
        /// Retorna uma lista com os comandos separados.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="groupRegex"></param>
        /// <returns></returns>
        private static List<string> GetGroupsCommands(string input, Regex groupRegex)
        {
            var regexMath = groupRegex.Match(input);
            var resultList = new List<string>();
            while (regexMath.Success)
            {
                resultList.Add(regexMath.Value);
                regexMath = regexMath.NextMatch();
            }
            return resultList;
        }

        /// <summary>
        /// Metodo Para cancelar os movimentos de acordo com a string enviada.
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private static string CancelMoviements(string res)
        {
            while (res.Contains("X"))
            {
                var indexX = res.IndexOf("X", StringComparison.Ordinal);
                if (int.TryParse(GetNumberReverse(res, indexX), out int re))
                {
                    res = res.Replace(re.ToString(), "");
                    indexX = res.IndexOf("X", StringComparison.Ordinal);
                }

                if (indexX != 0 && char.IsDigit(res[indexX - 1]))
                    res = res.Remove(indexX - 2, 3);
                else if (int.TryParse(GetNumber(res, indexX + 1), out int repeatx))
                    res = res.Remove(indexX - repeatx, 2 + repeatx);
                else if (indexX == 0)
                    res = res.Remove(indexX, 1);
                else
                    res = res.Remove(indexX - 1, 2);
            }
            return res;
        }


        /// <summary>
        /// Metodo para retornar o número após um charactere.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static string GetNumber(string input, int i)
        {
            if (input.Length > i && int.TryParse(input[i].ToString(), out var value))
            {
                return value + GetNumber(input, i + 1);
            }
            return "";
        }

        /// <summary>
        /// Metodo Retorna o número anterior se ouver a um character.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static string GetNumberReverse(string input, int i)
        {
            if (input.Length >= i && i != 0 && int.TryParse(input[i - 1].ToString(), out var value))
            {
                return GetNumberReverse(input, i - 1) + value;
            }
            return "";
        }

        /// <summary>
        /// Metodo para retornar o ponto em que o drone está.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        private class CartesianPlan
        {
            public CartesianPlan()
            {
            }

            private CartesianPlan(int x, int y)
            {
                X = x;
                Y = y;
            }

            private int X { get; set; }
            private int Y { get; set; }

            public void MoveDirection(string command) => MoveDirection(command, command.Length);

            public void MoveDirection(string command, int repeteValue)
            {
                for (var k = 0; k < repeteValue; k++)
                {
                    DecodeDiretion(command[0].ToString());
                }
            }

            private void DecodeDiretion(string direction)
            {
                switch (direction.ToUpper())
                {
                    case "N":
                        Y++;
                        break;
                    case "S":
                        Y--;
                        break;
                    case "O":
                        X--;
                        break;
                    case "L":
                        X++;
                        break;
                    default:
                        break;
                }
            }

            public static CartesianPlan WithPostion(int x, int y)
            {
                return new CartesianPlan(x, y);
            }

            public string GetPosition()
            {
                return $"({X}, {Y})";
            }
        }
    }
}
