using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Practices.Unity;
using WebGrease.Css.Extensions;

namespace ProjetoColaborativo.Mappers
{
    public class AutoMapperConfig
    {
        private const string MATCH_NAME = "ProjetoColaborativo";

        private static IEnumerable<Type> GetProfiles()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains(MATCH_NAME));

            var types = assemblies.SelectMany(i =>
                            i.GetTypes().Where(type =>
                                    typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract));
            return types;
        }

        public static void Register(IUnityContainer container)
        {
            GetProfiles().ForEach(profileType =>
            {
                Mapper.AddProfile(container.Resolve(profileType) as Profile);
            });
        }
    }
}