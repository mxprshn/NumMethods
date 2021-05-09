package laba3

import div
import get
import minus
import org.apache.commons.math3.linear.RealMatrix
import org.apache.commons.math3.linear.RealVector
import set
import times
import zeroVector
import kotlin.math.abs

class Lab3(val matrix: RealMatrix,
           val emergencyIterationsThreshold: Int,
           val initialVector: RealVector
) {
    class TooManyIterationsException: Throwable()

    private val ySequence = generateSequence(initialVector) { matrix * it }

    fun findMaxEigenvalueWithPowerMethod(epsilon: Double): List<Pair<Double, RealVector>> {
        val maxIndex = initialVector.map { abs(it) }.maxIndex

        return ySequence
            .map { it / it[maxIndex] }
            .zipWithNext { currentY, nextY ->
                val currentLambda = nextY[0] / currentY[0]
                currentLambda to currentY
            }
            .mapIndexed { index, pair ->
                if(index >= emergencyIterationsThreshold) throw TooManyIterationsException()
                pair
            }
            .takeWhile { (currentLambda, currentY) ->
                findPosteriorErrorEstimation(currentLambda, currentY) >= epsilon
            }
            .toList()
    }

    fun findMaxEigenvalueWithDotProductMethod(epsilon: Double): List<Pair<Double, RealVector>> {
        val maxIndex = initialVector.map { abs(it) }.maxIndex
        return ySequence
            .map { it / it[maxIndex] }
            .zipWithNext { currentY, nextY ->
                val currentLambda = nextY[0] / currentY[0]
                currentLambda to currentY
            }
            .mapIndexed { index, pair ->
                if(index >= emergencyIterationsThreshold) throw TooManyIterationsException()
                pair
            }
            .takeWhile { (currentLambda, currentY) ->
                findPosteriorErrorEstimation(currentLambda, currentY) >= epsilon
            }
            .toList()
    }

    private fun findPosteriorErrorEstimation(lambda: Double, eigenvector: RealVector): Double {
        return (matrix * eigenvector - lambda * eigenvector).norm / eigenvector.norm
    }
}