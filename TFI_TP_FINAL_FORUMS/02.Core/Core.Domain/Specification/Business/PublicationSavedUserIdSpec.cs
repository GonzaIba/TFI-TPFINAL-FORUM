using Core.Domain.Models;
using System.Linq.Expressions;

namespace Core.Domain.Specification.Business
{
    public class PublicationSavedUserIdSpec : Specification<PublicacionGuardadaModel>
    {
        private Specification<PublicacionGuardadaModel> userIdSpec;

        public PublicationSavedUserIdSpec(string userId)
        {
            userIdSpec = new AdHocSpecification<PublicacionGuardadaModel>(s => s.IDUsuario == userId);
        }

        public override Expression<Func<PublicacionGuardadaModel, bool>> ToExpression()
        {
            return userIdSpec;
        }
    }
}
