using AutoMapper;
using ProjetoColaborativo.Models.Entidades;
using ProjetoColaborativo.ViewModels.Usuario;

namespace ProjetoColaborativo.Mappers
{
    public class UsuarioProfileMap : Profile
    {
        protected override void Configure()
        {
           Mapper.CreateMap<Usuario, UsuarioViewModel>();

           Mapper.CreateMap<UsuarioViewModel, Usuario>();
        }
    }
}