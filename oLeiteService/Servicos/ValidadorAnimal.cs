using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using oLeiteService.Models;

namespace oLeiteService.Servicos
{
    public static class ValidadorAnimal
    {
        public static Boolean isDataFutura(DateTime? value, List<String> messages, String campo)
        {
            Boolean retorno = false;
            if (value != null && value.Value.CompareTo(DateTime.Now) > 0)
            {
                retorno = true;
                messages.Add($"{campo} não pode ser maior que hoje.");
            }
            return retorno;
        }

        public static Boolean isNull(DateTime? value, List<String> messages, String campo)
        {
            Boolean retorno = false;
            if (value == null)
            {
                retorno = true;
                messages.Add($"{campo} não pode ser vazio.");
            }
            return retorno;
        }

        public static Boolean isAnteriorNascimento(Animal animal, DateTime? value, List<String> messages, String campo)
        {
            Boolean retorno = false;
            if (value != null && value.Value.CompareTo(animal.nascimento.Value) < 0)
            {
                retorno = true;
                messages.Add($"{campo} não pode ser anterior {nameof(animal.nascimento)}.");
            }
            return retorno;
        }

        public static Boolean valorValido(long valor, List<String> messages, String campo)
        {
            Boolean retorno = true;
            if (valor <= 0)
            {
                retorno = false;
                messages.Add($"{campo} não pode ser menor que zero.");
            }
            return retorno;
        }

        public static Boolean valorValido(Double valor, List<String> messages, String campo)
        {
            Boolean retorno = true;
            if (valor <= 0)
            {
                retorno = false;
                messages.Add($"{campo} não pode ser menor que zero.");
            }
            return retorno;
        }
    }

}
