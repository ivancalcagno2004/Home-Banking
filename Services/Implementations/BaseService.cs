using HomeBanking.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Clase base para servicios de dominio/aplicación.
    /// Provee acceso compartido a <see cref="IUnitOfWork"/> para repositorios y persistencia.
    /// </summary>
    public class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
