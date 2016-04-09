using FluentNHibernate.Mapping;

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

            Map(x => x.Senha, "SENHA").Length(255);

            Map(x => x.Cpf, "CPF").Length(14);

            Map(x => x.Cor, "COR").Length(6);

            Map(x => x.Email, "EMAIL").Length(50);

            Map(x => x.Foto, "FOTO").Length(100);

            HasManyToMany(x => x.SessoesColaborativas).Cascade.All().Inverse().Table("USUARIOSESSAO");

            HasMany(x => x.Notificacoes).Cascade.AllDeleteOrphan();
        }
    }
}
