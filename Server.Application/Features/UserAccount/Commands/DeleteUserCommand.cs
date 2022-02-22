using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Server.Application.Contracts;

namespace Server.Application.Features.UserAccount.Commands
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public string Email { get; set; }

    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserAccount _userAccount;

        public DeleteUserCommandHandler(IUserAccount userAccount)
        {
            _userAccount = userAccount;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userAccount.DeleteUser(request.Email);
            return Unit.Value;
        }
    }
}