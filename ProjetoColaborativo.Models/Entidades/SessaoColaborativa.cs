﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoColaborativo.Models.Entidades
{
    public class SessaoColaborativa : Entidade
    {
        public virtual DateTime DataCriacao { get; set; }

        public virtual string Descricao { get; set; }

        public virtual List<Usuario> ListaUsuarios { get; set; }
    }
}
