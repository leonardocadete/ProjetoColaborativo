using System.Linq;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Business.Notificacao
{
    public class RepositorioNotificacao : IRepositorioNotificacao
    {
        private readonly IRepositorio<Models.Entidades.Usuario> repositorioUsuario;
        private readonly IRepositorio<SessaoColaborativa> repositorioSessao;
        private readonly IRepositorio<ObjetoSessao> repositorioObjeto; 

        public RepositorioNotificacao(
            IRepositorio<Models.Entidades.Usuario> repositorioUsuario, 
            IRepositorio<SessaoColaborativa> repositorioSessao, 
            IRepositorio<ObjetoSessao> repositorioObjeto)
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioSessao = repositorioSessao;
            this.repositorioObjeto = repositorioObjeto;
        }

        public void GerarNotificacao(long handleUsuarioLogado, long handleSessaoColaborativa, long handleObjetoSessao)
        {
            var sessaoColaborativa = repositorioSessao.Retornar(handleSessaoColaborativa);
            var objetoSessao = repositorioObjeto.Retornar(handleObjetoSessao);
            if (!EhUsuarioLogado(handleUsuarioLogado, sessaoColaborativa.Usuario))
                AdicionarNotificacao(sessaoColaborativa.Usuario, objetoSessao);
            foreach (var usuario in sessaoColaborativa.UsuariosDaSessao.Where(usuario => !EhUsuarioLogado(handleUsuarioLogado, usuario)))
                AdicionarNotificacao(usuario, objetoSessao);
        }

        public void FoiVisto(long handleUsuario, long handleObjetoSessao)
        {
            var usuario = ObterUsuario(handleUsuario);
            var notificacao = ObterNotificacao(usuario, handleObjetoSessao);
            if (notificacao != null)
                usuario.Notificacoes.Remove(notificacao);
            repositorioUsuario.Salvar(usuario);
        }

        private static void AdicionarNotificacao(Models.Entidades.Usuario usuario, ObjetoSessao objetoSessao)
        {
            var notificacaoExistente = usuario.Notificacoes.FirstOrDefault(x => x.ObjetoSessao == objetoSessao);
            if (notificacaoExistente != null)
                notificacaoExistente.Quantidade++;
            else
                usuario.Notificacoes.Add(new Models.Entidades.Notificacao
                {
                    Handle = 0,
                    ObjetoSessao = objetoSessao,
                    Quantidade = 1
                });
        }

        private static bool EhUsuarioLogado(long handleUsuarioLogado, Models.Entidades.Usuario usuario)
        {
            return usuario.Handle == handleUsuarioLogado;
        }

        private Models.Entidades.Usuario ObterUsuario(long handleUsuario)
        {
            return repositorioUsuario.Retornar(handleUsuario);
        }

        private static Models.Entidades.Notificacao ObterNotificacao(Models.Entidades.Usuario usuario, long handleObjetoSessao)
        {
            return usuario.Notificacoes.FirstOrDefault(x => x.ObjetoSessao.Handle == handleObjetoSessao);
        }
    }
}
