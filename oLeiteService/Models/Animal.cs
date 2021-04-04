using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using oLeiteService.Servicos;

namespace oLeiteService.Models
{
    public class Animal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String id { get; set; }
        public long numero { get; set; }
        public String nome { get; set; }
        private DateTime? _nascimento;
        public DateTime? nascimento {
            get { return _nascimento; }
            set { this._nascimento = value; }
        }

        public List<Inseminacao> inseminacoes { get; set; }
        public List<Pesagem> pesagens { get; set; }
        public List<Pesagem> pesagensLeite { get; set; }
        public List<Vacina> vacinas { get; set; }
        public List<Parto> partos { get; set; }
        public List<Ocorrencia> ocorrencias { get; set; }
        public List<Ccs> ccsLista { get; set; }

        public static readonly int DIAS_REPETIR_INSEMINACAO = 20;
        public static readonly int DIAS_OBSERVACAO = 20;
        public static readonly int DIAS_INSEMINAR_APOS_PARTO = 40;
        public static readonly int DIAS_GESTACAO = 270;
        public static readonly int IDADE_PARA_INSEMINAR_DIAS = 600;

        public long diasVida
        {
            get
            {
                if (this.nascimento == null) return 0L;
                TimeSpan difference = DateTime.Now - (DateTime)this.nascimento;
                return (long)difference.TotalDays;
            }
        }

        public Boolean estaParaInseminar
        {
            get
            {
                Boolean isSelecionado = false;
                if (this.ultimoParto == null && temIdadeParaInseminar)
                {
                    isSelecionado = !estaInseminada;
                }
                else if (isUltimoPartoAntigo)
                {
                    isSelecionado = !estaInseminada;
                }
                return isSelecionado;
            }
        }

        public Boolean estaInseminada
        {
            get
            {
                Boolean isInseminada = false;
                Inseminacao _inseminacao = this.ultimaInseminacao;
                if (_inseminacao == null) return false;
                Parto ultimoParto = this.ultimoParto;
                if (ultimoParto == null || _inseminacao.data.CompareTo(ultimoParto.data) > 0) {
                    if (_inseminacao.confirmado) {
                        isInseminada = true;
                    }
                    else if (_inseminacao.data.CompareTo(DateTime.Now.AddDays(-DIAS_REPETIR_INSEMINACAO)) > 0) {
                        isInseminada = true;
                    }
                }
                return isInseminada;
            }
        }

        public Boolean temIdadeParaInseminar => (this.diasVida > IDADE_PARA_INSEMINAR_DIAS);

        public Boolean isUltimoPartoAntigo
        {
            get
            {
                Parto ultimoParto = this.ultimoParto;
                return (ultimoParto != null && ultimoParto.data.CompareTo(DateTime.Now.AddDays(-DIAS_INSEMINAR_APOS_PARTO)) < 0);
            }
        }

        public Boolean isObservacaoParto
        {
            get
            {
                Parto _ultimoParto = this.ultimoParto;
                Inseminacao _ultimaInseminacao = this.ultimaInseminacao;
                Boolean verificacaoPosParto = (_ultimoParto != null && _ultimoParto.data.CompareTo(DateTime.Now.AddDays(-DIAS_OBSERVACAO)) > 0);
                Boolean verificacaoPreParto = (
                    this.estaInseminada
                        && _ultimaInseminacao.data.AddDays(DIAS_GESTACAO).CompareTo(DateTime.Now.AddDays(-DIAS_OBSERVACAO)) > 0);

                return verificacaoPosParto || verificacaoPreParto;
            }
        }

        public Inseminacao ultimaInseminacao
        {
            get
            {
                Inseminacao _ultimaInseminacao = null;
                foreach (Inseminacao inseminacao in this.inseminacoes)
                {
                    if ((_ultimaInseminacao == null && inseminacao.data != null)
                        || (inseminacao.data != null && _ultimaInseminacao.data != null
                            && inseminacao.data.CompareTo(_ultimaInseminacao.data) > 0))
                    {
                        _ultimaInseminacao = inseminacao;
                    }
                }
                Inseminacao retorno = null;
                if (_ultimaInseminacao != null) {
                    retorno = new Inseminacao();
                    retorno.confirmado = _ultimaInseminacao.confirmado;
                    retorno.data = _ultimaInseminacao.data;
                    retorno.id = _ultimaInseminacao.id;
                }
                return retorno;
            }
        }

        public Parto ultimoParto
        {
            get
            {
                Parto _ultimoParto = null;
                foreach (Parto parto in this.partos)
                {
                    if ((_ultimoParto == null && parto.data != null)
                        || (parto.data != null && _ultimoParto.data != null
                            && parto.data.CompareTo(_ultimoParto.data) > 0))
                    {
                        _ultimoParto = parto;
                    }
                }
                Parto retorno = null;
                if (_ultimoParto != null)
                {
                    retorno = new Parto();
                    retorno.observacao = _ultimoParto.observacao;
                    retorno.data = _ultimoParto.data;
                    retorno.id = _ultimoParto.id;
                }
                return retorno;
            }
        }

        public Boolean estaValido(List<String> messages)
        {
            Boolean isValid = true;
            isValid = isValid && !ValidadorAnimal.isNull(this.nascimento, messages, "Nascimento");
            isValid = isValid && !ValidadorAnimal.isDataFutura(this.nascimento, messages, "Nascimento");

            // Validações das listas relacionadas ao animal
            this.ccsLista.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.inseminacoes.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.ocorrencias.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.partos.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.pesagens.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.pesagensLeite.ForEach(item => isValid = isValid && item.estaValido(this, messages));
            this.vacinas.ForEach(item => isValid = isValid && item.estaValido(this, messages));

            return isValid;
        }

    }

    public abstract class elementoComData
    {
        public long id { get; set; }
        private DateTime _data;
        protected String nomeObjeto;
        public DateTime data
        {
            get { return _data; }
            set { this._data = value; }
        }
        public Boolean estaValido(Animal animal, List<String> messages)
        {
            Boolean isValid = true;
            isValid = isValid && !ValidadorAnimal.isNull(this.data, messages, "Data " + this.GetType().ToString());
            isValid = isValid && !ValidadorAnimal.isDataFutura(this.data, messages, "Data" + this.GetType().ToString());
            isValid = isValid && !ValidadorAnimal.isAnteriorNascimento(animal, this.data, messages, "Data" + this.GetType().ToString());
            return isValid;
        }
    }

    public class Ccs: elementoComData
    {
        public long valor { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            return base.estaValido(animal, messages) 
                && ValidadorAnimal.valorValido(this.valor, messages, "Valor " + this.GetType().ToString());
        }
    }

    public class Inseminacao : elementoComData
    {
        public Boolean confirmado { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            return base.estaValido(animal, messages);
        }
    }

    public class Ocorrencia : elementoComData
    {
        public String nome { get; set; }
        public String observacao { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            Boolean isValid = true;
            isValid = isValid && base.estaValido(animal, messages);
            if (this.nome == null || this.nome.Trim().Length == 0) { 
                isValid = false;
                messages.Add("O nome da ocorrência deve ser informado");
            }
            return isValid;
        }
    }

    public class Parto : elementoComData
    {
        public String observacao { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            return base.estaValido(animal, messages);
        }
    }

    public class Pesagem : elementoComData
    {
        public Double peso { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            return base.estaValido(animal, messages)
                && ValidadorAnimal.valorValido(this.peso, messages, "Peso " + this.GetType().ToString());
        }
    }

    public class Vacina : elementoComData
    {
        public String nome { get; set; }
        public new Boolean estaValido(Animal animal, List<String> messages)
        {
            return base.estaValido(animal, messages);
        }
    }

}
