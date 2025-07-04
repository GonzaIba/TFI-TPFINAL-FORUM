using Core.Domain.Models;
using System.Linq.Expressions;

namespace Core.Domain.Specification.Business
{
    public class PublicationUserIdSpec : Specification<PublicacionModel>
    {
        private Specification<PublicacionModel> userIdSpec;

        public PublicationUserIdSpec(string userId)
        {
            userIdSpec = new AdHocSpecification<PublicacionModel>(s => s.IDUsuario == userId);
        }

        public override Expression<Func<PublicacionModel, bool>> ToExpression()
        {
            return userIdSpec;
        }
    }
}
