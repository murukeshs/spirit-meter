using AuthorizedServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizedServer.Repositories
{
    public interface IRTokenRepository
    {

        bool AddToken(RToken token);

        bool ExpireToken(RToken token);

        RToken GetToken(string refresh_token, string client_id);
    }
}
