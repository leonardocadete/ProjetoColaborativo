using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ProjetoColaborativo.Models.DAO;
using ProjetoColaborativo.Models.Entidades;

namespace ProjetoColaborativo.Store
{
    public class UserStore<TUser> : 
        IUserStore<TUser>,
        IUserLockoutStore<TUser, string>,
        IUserPasswordStore<TUser>,
        IUserTwoFactorStore<TUser, string>
        where TUser : Usuario 
    {
        private readonly IRepositorio<TUser> repositorio;

        public UserStore(IRepositorio<TUser> repositorio)
        {
            this.repositorio = repositorio;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && repositorio != null && repositorio.Sessao != null)
                repositorio.Sessao.Dispose();
        }

        public Task CreateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            repositorio.Salvar(user);
            repositorio.Sessao.Flush();
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            repositorio.Sessao.Clear();
            repositorio.Salvar(user);

            return Task.FromResult(0);
        }

        public Task DeleteAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            using (var transaction = repositorio.Sessao.BeginTransaction())
            {
                repositorio.Excluir(user);
                transaction.Commit();
            }
            repositorio.Sessao.Flush();
            repositorio.Sessao.Clear();

            return Task.FromResult(0);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return Task.FromResult(repositorio.Retornar(Convert.ToInt64(userId)));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(repositorio.Consultar(x => x.Login == userName).FirstOrDefault());
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(true);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Senha = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Senha);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(user.Senha != null);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(false);
        }
    }
}