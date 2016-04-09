namespace ProjetoColaborativo.Models.Entidades
{
    public class Notificacao : Entidade
    {
        public virtual ObjetoSessao ObjetoSessao { get; set; }

        public virtual int Quantidade { get; set; }
    }
}
