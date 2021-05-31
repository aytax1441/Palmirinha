using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receitas
{
    class receitas
    {
        public int codigo {get; }
        public string autor { get; set; }
        public string titulo { get; set; }
        public DateTime dta_criacao { get; }
        public DateTime dta_alteracao { get; set; }
        public string receita { get; set; }

        public receitas(string cod, string tit, string aut, string criacao, string alteracao, string rec)
        {
            int newCod = Convert.ToInt32(cod);
            codigo = newCod;
            autor = aut;
            titulo = tit;
            DateTime new_dta_criacao = Convert.ToDateTime(criacao);
            dta_criacao = new_dta_criacao;
            DateTime new_dta_alteracao = Convert.ToDateTime(alteracao);
            dta_alteracao = new_dta_alteracao;
            receita = rec;
        }
    }
}
