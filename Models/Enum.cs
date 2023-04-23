using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigimonDB.Models
{
    public enum CLTYPE
    {
        NONE,
        THEME_BOOSTER,
        BOOSTER,
        REBOOT_BOOSTER,
        STARTER_DECK,
        ADVANCE_DECK
    }

    public enum EFTYPE
    {
        SIMPLE,
        DIGIEVOLVE,
        SECURITY
    }

    public enum STATUS
    {
        NONE,
        NEW,
        UPDATE,
        DELETE
    }
}
