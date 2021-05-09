import org.apache.commons.math3.linear.ArrayRealVector
import org.apache.commons.math3.linear.MatrixUtils
import org.apache.commons.math3.linear.RealMatrix
import org.apache.commons.math3.linear.RealVector

fun arrayOfZeros(size: Int) = DoubleArray(size) { 0.0 }
fun zeroMatrix(size: Int): RealMatrix = MatrixUtils.createRealMatrix(Array(size) { arrayOfZeros(size) })
fun zeroVector(size: Int): RealVector = ArrayRealVector(arrayOfZeros(size))