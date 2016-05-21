using AutoMapper;
using ProjetoColaborativo.Business.Usuario.ViewModels;
using ProjetoColaborativo.Models.Entidades;

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