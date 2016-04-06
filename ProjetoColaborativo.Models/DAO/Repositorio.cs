using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Models.DAO
{
    public class Repositorio<T> : IDisposable, IRepositorio<T> where T : Entidade
    {
        private readonly ISession session;
        public Guid guid = Guid.NewGuid();

        public Repositorio(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            this.session = session;
        }

        public ISession Sessao
        {
            get
            {
                return session;
            }
        }

        public virtual T Incluir(T entity)
        {
            if (entity == default(T))
                throw new ArgumentNullException("entity");
            Sessao.Save(entity);

            return entity;
        }

        public virtual void IncluirVarios(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            foreach (var item in entities)
                Incluir(item);
        }

        public virtual T Alterar(T entity)
        {
            return Salvar(entity);
        }

        public virtual T Salvar(T entity)
        {
            if (entity == default(T))
                throw new ArgumentNullException("entity");

            if (entity.Handle > 0)
                return Sessao.Merge(entity);

            Sessao.SaveOrUpdate(entity);
            return entity;
        }

        public virtual IQueryable<T> SalvarVarios(IQueryable<T> listEntity)
        {
            if (listEntity == null)
                throw new ArgumentNullException("listEntity");

            var savedList = new List<T>();

            foreach (var entity in listEntity)
            {
                savedList.Add(Salvar(entity));
            }

            return savedList.AsQueryable();
        }

        public virtual void Excluir(T entity)
        {
            if (entity == default(T))
                throw new ArgumentNullException("entity");

            Sessao.Delete(entity);
        }

        public virtual void ExcluirVarios(IEnumerable<T> entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            foreach (var item in entity)
                Excluir(item);
        }

        public virtual void ExcluirVarios(Expression<Func<T, bool>> where)
        {
            var entitys = this.Consultar(where);
            foreach (var item in entitys)
                Excluir(item);
        }

        public void EvitarAlteracoes(T entity)
        {
            if (entity != null)
                session.Evict(entity);
        }

        public T Retornar(long handle)
        {
            if (handle == 0)
                return null;
            return Sessao.Get<T>(handle);
        }

        public IQueryable<T> RetornarTodos()
        {
            return (from t in Sessao.Query<T>()
                    select t);
        }

        public IQueryable<T> Consultar(Expression<Func<T, bool>> where = null)
        {
            var result = RetornarTodos();
            if (where != null)
                result = result.Where(where);
            return result;
        }

        public void Dispose()
        {
            if (session.IsOpen)
            {
                Sessao.Flush();
                Sessao.Close();
            }
        }

        public void Recarregar(T entity)
        {
            if (entity == default(T))
                throw new ArgumentNullException("entity");

            Sessao.Refresh(entity);
        }
    }
}
