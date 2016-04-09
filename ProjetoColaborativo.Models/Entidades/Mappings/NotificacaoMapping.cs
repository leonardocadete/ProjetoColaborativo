using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Entidades.Mappings
{
    public class NotificacaoMapping : ClassMap<Notificacao>
    {
        public NotificacaoMapping()
        {
            Table("NOTIFICACAO");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
                builder => builder.AddParam("sequence", "SEQ_NOTIFICACAO"));

            References(x => x.ObjetoSessao);

            Map(x => x.Quantidade);
        }
    }
}
