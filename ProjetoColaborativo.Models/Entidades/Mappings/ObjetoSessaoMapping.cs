using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Entidades.Mappings
{
    public class ObjetoSessaoMapping : ClassMap<ObjetoSessao>
    {
        public ObjetoSessaoMapping()
        {
            Table("OBJETOSESSAO");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
                builder => builder.AddParam("sequence", "SEQ_OBJETOSESSAO"));

            Map(x => x.UrlImagem, "URLIMAGEM").Length(100);
            Map(x => x.DataCriacao, "DATACRIACAO").Not.Nullable();

            HasMany(x => x.ElementosMultimidia).KeyColumn("OBJETOSESSAO").Cascade.AllDeleteOrphan();
        }
    }
}
