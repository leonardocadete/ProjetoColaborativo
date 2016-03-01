using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace ProjetoColaborativo.Models.Entidades.Mappings
{
    public class ElementoMultimidiaMapping : ClassMap<ElementoMultimidia>
    {
        public ElementoMultimidiaMapping()
        {
            Table("ELEMENTOMULTIMIDIA");

            Id(x => x.Handle, "ID").Not.Nullable().GeneratedBy.Native(
               builder => builder.AddParam("sequence", "SEQ_ELEMENTOMULTIMIDIA"));

            Map(x => x.DataCriacao, "DATACRIACAO").Not.Nullable();
            Map(x => x.Json, "JSON").Not.Nullable().Length(10000);
            Map(x => x.Guid, "GUID").Not.Nullable();

            References(x => x.ObjetoSessao).Column("OBJETOSESSAO").Cascade.All();

        }
    }
}
