using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Mappings
{
    public class UsuarioMapping : ClassMap<Usuario>
    {
        public UsuarioMapping()
        {
            Table("USUARIO");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
                builder => builder.AddParam("sequence", "SEQ_USUARIO"));

            Map(x => x.Nome, "NOME");
        }
    }
}
