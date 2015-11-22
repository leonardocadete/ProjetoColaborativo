using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Models.DAO
{
    public interface IRepositorio<T> where T : Entidade
    {
        ISession Sessao { get; }

        T Incluir(T entity);

        void IncluirVarios(IEnumerable<T> entity);

        T Alterar(T entity);

        void Recarregar(T entity);

        T Salvar(T entity);

        IQueryable<T> SalvarVarios(IQueryable<T> listEntity);

        void Excluir(T entity);

        void ExcluirVarios(IEnumerable<T> entity);

        void ExcluirVarios(Expression<Func<T, bool>> where);

        void EvitarAlteracoes(T entity);

        /// <summary>
        /// Retorna a Entidade através do handle informado
        /// </summary>
        /// <param name="handle">Handle identificador da entidade</param>
        /// <returns>Entidade localizada. Nulo caso não encontrada.</returns>
        T Retornar(long handle);

        /// <summary>
        /// Retorna todas as Entidades do tipo
        /// </summary>
        /// <returns>Queriable que permite percorrer todas as instâncias ou filtrar o resultado ainda mais.</returns>
        IQueryable<T> RetornarTodos();

        /// <summary>
        /// Consulta Todas as Entidades de acordo com condição where
        /// </summary>
        /// <param name="where">Predicado para filtragem</param>
        /// <returns>Queriable que permite percorrer todas as instâncias ou filtrar o resultado ainda mais.</returns>
        IQueryable<T> Consultar(Expression<Func<T, bool>> where = null);
    }
}
