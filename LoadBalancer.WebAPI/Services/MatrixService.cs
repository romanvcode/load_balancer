namespace LoadBalancer.WebAPI.Services;

public class MatrixService
{
    public static double[][] GenerateMatrix(int size)
    {
        var rand = new Random();
        double[][] matrix = new double[size][];

        for (int i = 0; i < size; i++)
        {
            matrix[i] = new double[size];
            for (int j = 0; j < size; j++)
            {
                matrix[i][j] = rand.Next(1, 100);
            }
        }

        return matrix;
    }

    public static double[] GenerateVector(int size)
    {
        var rand = new Random();
        double[] vector = new double[size];

        for (int i = 0; i < size; i++)
        {
            vector[i] = rand.Next(1, 100);
        }

        return vector;
    }
}
