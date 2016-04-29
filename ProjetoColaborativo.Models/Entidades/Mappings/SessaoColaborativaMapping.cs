using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Entidades.Mappings
{
    public class SessaoColaborativaMapping : ClassMap<SessaoColaborativa>
    {
        public SessaoColaborativaMapping()
        {
            Table("SESSAOCOLABORATIVA");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
                builder => builder.AddParam("sequence", "SEQ_SESSAOCOLABORATIVA"));

            Map(x => x.DataCriacao, "DATACRIACAO").Not.Nullable();
            Map(x => x.Descricao, "DESCRICAO").Not.Nullable().Length(100);
            Map(x => x.Fechada, "FECHADA").Default("0");
            Map(x => x.Arquivada, "ARQUIVADA").Default("0");

            References(x => x.Usuario).Column("USUARIO");

            HasMany(x => x.ObjetosDaSessao).KeyColumn("SESSAOCOLABORATIVA").Cascade.AllDeleteOrphan();

            HasManyToMany(x => x.UsuariosDaSessao).Cascade.All().Table("USUARIOSESSAO");
        }
    }
}
