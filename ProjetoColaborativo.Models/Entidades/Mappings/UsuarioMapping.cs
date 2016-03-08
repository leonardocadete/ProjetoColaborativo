﻿using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Entidades.Mappings
{
    public class UsuarioMapping : ClassMap<Usuario>
    {
        public UsuarioMapping()
        {
            Table("USUARIO");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
                builder => builder.AddParam("sequence", "SEQ_USUARIO"));

            Map(x => x.Nome, "NOME").Length(100);

            Map(x => x.Login, "LOGIN").Length(20);

            Map(x => x.Senha, "SENHA").Length(20);

            Map(x => x.Cpf, "CPF").Length(14);

            Map(x => x.Cor, "COR").Length(6);

            HasMany(x => x.ElementosMultimidia).KeyColumn("USUARIO").Cascade.AllDeleteOrphan();
        }
    }
}
