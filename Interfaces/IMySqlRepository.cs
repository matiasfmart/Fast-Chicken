using System;
using FastChicken.Models;

namespace FastChicken.Interfaces
{
	public interface IMySqlRepository
    {
        List<Combo> GetCombos();
    }
}