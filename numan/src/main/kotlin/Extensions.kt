import org.apache.commons.math3.linear.*

operator fun RealMatrix.minus(other: RealMatrix): RealMatrix = this.subtract(other)
operator fun RealMatrix.times(other: RealMatrix): RealMatrix = this.multiply(other)
operator fun RealMatrix.times(vector: RealVector): RealVector = this.preMultiply(vector)
operator fun RealMatrix.get(row: Int, column: Int): Double = this.getEntry(row, column)
operator fun RealMatrix.set(row: Int, column: Int, value: Double) = this.setEntry(row, column, value)
fun RealMatrix.getLastRow(index: Int): RealVector = this.getRowVector(this.rowDimension - index - 1)
fun RealMatrix.getLastRow(): RealVector = this.getLastRow(0)

fun RealMatrix.computeEigenValues(): DoubleArray {
    val decomposition = EigenDecomposition(this)
    return decomposition.realEigenvalues
}

fun RealMatrix.computeSpectralRadius(): Double {
    val eigenValues = this.computeEigenValues()
    eigenValues.sort()
    return eigenValues.last()
}

fun RealMatrix.computeInverse(): RealMatrix = LUDecomposition(this).solver.inverse

operator fun Double.times(matrix: RealMatrix): RealMatrix = matrix.scalarMultiply(this)
operator fun Double.times(vector: RealVector): RealVector = vector.mapMultiply(this)

operator fun RealVector.get(index: Int): Double = this.getEntry(index)
operator fun RealVector.set(index: Int, value: Double) = this.setEntry(index, value)
operator fun RealVector.minus(other: RealVector): RealVector = this.subtract(other)
operator fun RealVector.plus(other: RealVector): RealVector = this.add(other)
operator fun RealVector.div(scalar: Double): RealVector = 1 / scalar * this
fun RealVector.normalize(): RealVector {
    val norm = this.norm
    return this.map { it / norm }
}
infix fun RealVector.dot(other: RealVector): Double = this.dotProduct(other)

fun DoubleArray.toVector() = ArrayRealVector(this)
fun Array<DoubleArray>.toMatrix(): RealMatrix = MatrixUtils.createRealMatrix(this)