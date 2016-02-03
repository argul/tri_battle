using System;
using System.Collections.Generic;

// TODO : spawn strategy
public class SlotFactory2D_Rectangular
{
	protected Randomizer rand = new Randomizer();
	protected SlotConfig config;
	public SlotFactory2D_Rectangular(SlotConfig config)
	{
		this.config = config;
		rand.Seed(config.randomSeed);
		retangularAdjacencies[0] = new SlotAdjacencyInfo2D_Rectangular(SlotAdjacencyInfo2D_Rectangular.Direction.LEFT);
		retangularAdjacencies[1] = new SlotAdjacencyInfo2D_Rectangular(SlotAdjacencyInfo2D_Rectangular.Direction.RIGHT);
		retangularAdjacencies[2] = new SlotAdjacencyInfo2D_Rectangular(SlotAdjacencyInfo2D_Rectangular.Direction.UP);
		retangularAdjacencies[3] = new SlotAdjacencyInfo2D_Rectangular(SlotAdjacencyInfo2D_Rectangular.Direction.DOWN);
	}
	private SlotAdjacencyInfo[] retangularAdjacencies = new SlotAdjacencyInfo[4];

	public SlotAttribute ProduceInsulator()
	{
		return Producer(SlotAttribute.Category.INSULATOR);
	}

	public SlotAttribute ProduceSubject()
	{
		return Producer(SlotAttribute.Category.TARGET);
	}

	public SlotAttribute Producer(SlotAttribute.Category category)
	{
		switch (category)
		{
		case SlotAttribute.Category.INSULATOR:
			return Producer_INSULATOR();
		case SlotAttribute.Category.TARGET:
			return Producer_TARGET();
		default:
			throw new NotSupportedException();
		}
	}

	protected SlotAttribute Producer_INSULATOR()
	{
		var slot = new SlotAttribute(SlotAttribute.Category.INSULATOR);
		slot.adjacencies = retangularAdjacencies;
		return slot;
	}

	protected SlotAttribute Producer_TARGET()
	{
		var slot = new SlotAttribute(SlotAttribute.Category.TARGET);
		slot.trait = config.Traits[rand.NextInt(0, config.Traits.Count)];
		slot.adjacencies = retangularAdjacencies;
		return slot;
	}
}