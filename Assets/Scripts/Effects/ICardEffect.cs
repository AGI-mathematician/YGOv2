using UnityEngine;
using System.Threading.Tasks;

public interface ICardEffect {
	Task Resolve(ChainLink link);
}
