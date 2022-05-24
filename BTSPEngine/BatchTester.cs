namespace BTSPEngine
{
	public class BatchTester
	{
		private readonly BTSPGenerator generator;

		public BatchTester(BTSPGenerator generator)
		{
			this.generator = generator;
		}

		public void RunTests(int fromSize, int toSize, int sizeStep, int samplesPerSize, string reportPath)
		{
			var solver = new BTSPSolver();

			var sizesCount = (toSize - fromSize) / sizeStep + 1;
			if (fromSize + (sizesCount - 1) * sizeStep != toSize)
				throw new ArgumentException("Invalid step");

			using (var fileWriter = new StreamWriter(new FileStream(reportPath, FileMode.Create)))
			{
				fileWriter.WriteLine("Size,Max weigth,BST max weight,Solution max weight,Solution valid");

				for (int i = 0; i < sizesCount; i++)
				{
					var size = fromSize + i * sizeStep;

					for (int j = 0; j < samplesPerSize; j++)
					{
						var btsp = generator.Generate(size);
						var maxEdgeWeight = btsp.Edges.Max(e => e.Weight);
						var solution = solver.SolveBTSP(btsp);
						var solutionValid = solution.CycleMaxEdgeWeight <= 3 * solution.BSTMaxEdgeWeight;

						fileWriter.WriteLine(
							$"{size},{maxEdgeWeight},{solution.BSTMaxEdgeWeight}," +
							$"{solution.CycleMaxEdgeWeight},{solutionValid}"
						);
					}
				}
			}
		}
	}
}