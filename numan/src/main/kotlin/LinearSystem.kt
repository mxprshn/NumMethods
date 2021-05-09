import org.apache.commons.math3.linear.EigenDecomposition
import org.apache.commons.math3.linear.RealMatrix
import org.apache.commons.math3.linear.RealVector

data class LinearSystem(val matrix: RealMatrix, val consts: RealVector) {
    val dimension = matrix.rowDimension

    init {
        require(matrix.isSquare)
        require(consts.dimension == dimension)
    }
}