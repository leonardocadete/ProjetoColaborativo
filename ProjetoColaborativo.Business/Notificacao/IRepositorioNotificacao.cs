namespace ProjetoColaborativo.Business.Notificacao
{
    public interface IRepositorioNotificacao
    {
        void GerarNotificacao(long handleUsuarioLogado, long handleSessaoColaborativa, long handleObjetoSessao);
        void FoiVisto(long handleUsuario, long handleObjetoSessao);
    }
}
