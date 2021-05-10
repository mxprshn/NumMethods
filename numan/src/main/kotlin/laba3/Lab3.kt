package laba3

import dot
import get
import minus
import normalize
import org.apache.commons.math3.linear.EigenDecomposition
import org.apache.commons.math3.linear.RealMatrix
import org.apache.commons.math3.linear.RealVector
import times
import java.lang.IllegalStateException
import kotlin.math.abs

class Lab3(val matrix: RealMatrix,
           val emergencyIterationsThreshold: Int,
           val initialVector: RealVector
) {
    class TooManyIterationsException: Throwable()

    data class IterationResult(val lambda: Double, val eigenvector: RealVector, val posteriorEstimation: Double)

    val exactEigenvalues by lazy {
        val eigenvalues = eigenDecomposition.realEigenvalues
        eigenvalues
    }

    val exactNormalizedEigenvector by lazy {
        val maxIndex = exactEigenvalues.withIndex().maxByOrNull { abs(it.value) }?.index ?:
            throw IllegalStateException("Cannot find index of max eigenvalue")
        eigenDecomposition.getEigenvector(maxIndex).normalize()
    }

    val exactMaxAbsEigenValue by lazy { sortedEigenvalueAbs.last() }
    val lambda2OverLambda1 by lazy { sortedEigenvalueAbs[sortedEigenvalueAbs.size - 2] / exactMaxAbsEigenValue }

    private val sortedEigenvalueAbs by lazy { exactEigenvalues.sortedBy { abs(it) } }
    private val eigenDecomposition by lazy { EigenDecomposition(matrix) }

    fun findMaxEigenvalueWithPowerMethod(epsilon: Double): List<IterationResult> {
        var prevY = matrix * initialVector
        var currentY = matrix * prevY
        var posteriorEstimation: Double
        val result = mutableListOf<IterationResult>()
        var iterationCount = 0

        do {
            val currentLambda = currentY[1] / prevY[1]
            posteriorEstimation = findPosteriorErrorEstimation(currentLambda, prevY)
            result.add(IterationResult(currentLambda, currentY.normalize(), posteriorEstimation))
            prevY = currentY
            currentY = matrix * currentY
            ++iterationCount
            check(iterationCount < emergencyIterationsThreshold)
        } while(posteriorEstimation >= epsilon)

        return result
    }

    fun findMaxEigenvalueWithDotProductMethod(epsilon: Double): List<IterationResult> {
        var prevY = initialVector
        var currentY = matrix * initialVector
        var posteriorEstimation: Double
        val result = mutableListOf<IterationResult>()
        var iterationCount = 0

        do {
            val currentLambda = (currentY dot prevY) / (prevY dot prevY)
            posteriorEstimation = findPosteriorErrorEstimation(currentLambda, prevY)
            result.add(IterationResult(currentLambda, currentY.normalize(), posteriorEstimation))
            prevY = currentY
            currentY = matrix * currentY
            ++iterationCount
            check(iterationCount < emergencyIterationsThreshold)
        } while(posteriorEstimation >= epsilon)

        return result
    }

    private fun findPosteriorErrorEstimation(lambda: Double, eigenvector: RealVector): Double {
        return (matrix * eigenvector - lambda * eigenvector).norm / eigenvector.norm
    }
}