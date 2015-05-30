﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Data
{
    public class RoleRepository : IRoleRepository, IDisposable
    {
        private readonly RoleStore<IdentityRole> _store;
        private readonly RoleManager<IdentityRole> _manager;

        public RoleRepository()
        {
            _store = new RoleStore<IdentityRole>();
            _manager = new RoleManager<IdentityRole>(_store);
        }

        public IdentityRole GetRoleByName(string name)
        {
            return _store.FindByNameAsync(name).Result;
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _store.Roles.ToArray();
        }

        public void Create(IdentityRole role)
        {
            _manager.Create(role);
        }

        private bool _disposed;

        public void Dispose()
        {
            if(!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}
