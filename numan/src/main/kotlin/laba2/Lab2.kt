package laba2

import arrayOfZeros
import computeEigenValues
import computeInverse
import computeSpectralRadius
import get
import getLastRow
import minus
import org.apache.commons.math3.linear.*
import plus
import set
import times
import toMatrix
import zeroMatrix
import zeroVector
import kotlin.math.pow
import kotlin.math.sqrt

class Lab2(val sourceMatrix: RealMatrix, val sourceConstVector: RealVector) {

    val sourceEigenValues by lazy { sourceMatrix.computeEigenValues() }

    val exactSolution: RealVector by lazy {
        val solver = LUDecomposition(sourceMatrix).solver
        solver.solve(sourceConstVector)
    }

    val optimalAlpha by lazy {
        sourceEigenValues.sort()
        2 / (sourceEigenValues.first() + sourceEigenValues.last())
    }

    private val alphaOptimalSystem by lazy { transformSystemToAlphaOptimal() }
    val alphaOptimalH get() = alphaOptimalSystem.first
    val alphaOptimalG get() = alphaOptimalSystem.second

    val jacobiOptimalSystem by lazy { transformSystemToJacobiOptimal() }
    val jacobiOptimalH get() = jacobiOptimalSystem.first
    val jacobiOptimalG get() = jacobiOptimalSystem.second

    val seidelTransformationMatrix by lazy { findSeidelTransformationMatrix() }
    val optimalQForUpperRelaxationMethod by lazy { 2 / (1 + sqrt(1 - jacobiOptimalSpectralRadius.pow(2))) }

    private val systemDimension get() = sourceMatrix.rowDimension
    private val jacobiOptimalSpectralRadius by lazy { jacobiOptimalSystem.first.computeSpectralRadius() }
    private val alphaOptimalSpectralRadius by lazy { alphaOptimalSystem.first.computeSpectralRadius() }
    private val seidelTransformationSpectralRadius by lazy { seidelTransformationMatrix.computeSpectralRadius() }

    fun findSolutionsWithIterMethod(i: Int): RealMatrix {
        val solutions = Array(i + 1) { arrayOfZeros(systemDimension) }
        for(j in 1..i) {
            for(k in 0 until systemDimension) {
                var sum = 0.0
                for(l in 0 until systemDimension) {
                    sum += alphaOptimalH[k, l] * solutions[j - 1][k]
                }
                sum += alphaOptimalG[k]
                solutions[j][k] = sum
            }

        }
        return solutions.toMatrix()
    }

    fun findIterPosteriorEstimation(iterSolutions: RealMatrix): Double {
        return alphaOptimalH.norm / (1.0 - alphaOptimalH.norm) *
            (iterSolutions.getLastRow(0) - iterSolutions.getLastRow(1)).norm
    }

    fun refineIterSolutionWithLyusternikMethod(iterSolutions: RealMatrix): RealVector =
        refineWithLyusternikMethod(alphaOptimalSpectralRadius, iterSolutions)

    fun findSolutionsWithSeidelMethod(i: Int): RealMatrix {
        val solutions = Array(i + 1) { arrayOfZeros(systemDimension) }
        for(j in 1..i) {
            for(k in 0 until systemDimension) {
                var sum = 0.0
                for(l in k until systemDimension) {
                    sum += alphaOptimalH[k, l] * solutions[j - 1][l]
                }
                for(l in 0 until k) {
                    sum += alphaOptimalH[k, l] * solutions[j][l]
                }
                solutions[j][k] = sum + alphaOptimalG[k]
            }
        }
        return solutions.toMatrix()
    }

    fun refineSeidelSolutionWithLyusternikMethod(seidelSolutions: RealMatrix): RealVector =
        refineWithLyusternikMethod(seidelTransformationSpectralRadius, seidelSolutions)

    fun findSolutionsWithUpperRelaxationMethod(q: Double, i: Int): RealMatrix {
        val solutions = Array(i + 1) { arrayOfZeros(systemDimension) }
        for(j in 1..i) {
            for(k in 0 until systemDimension) {
                var sum = 0.0
                for(l in k + 1 until systemDimension) {
                    sum += jacobiOptimalH[k, l] * solutions[j - 1][l]
                }
                for(l in 0 until k) {
                    sum += jacobiOptimalH[k, l] * solutions[j][l]
                }
                solutions[j][k] = solutions[j - 1][k] + q * (sum - solutions[j - 1][k] + jacobiOptimalG[k])
            }
        }
        return solutions.toMatrix()
    }

    private fun transformSystemToAlphaOptimal(): Pair<RealMatrix, RealVector> {
        val ones = MatrixUtils.createRealIdentityMatrix(systemDimension)
        val transformedMatrix = ones - optimalAlpha * sourceMatrix
        val transformedConstVector = optimalAlpha * sourceConstVector
        return transformedMatrix to transformedConstVector
    }

    private fun transformSystemToJacobiOptimal(): Pair<RealMatrix, RealVector> {
        val transformedMatrix = zeroMatrix(systemDimension)
        val transformedConstVector = zeroVector(systemDimension)
        for(i in 0 until systemDimension) {
            transformedConstVector[i] = sourceConstVector[i] / sourceMatrix[i, i]
            for(j in 0 until systemDimension) {
                transformedMatrix[i, j] = if(i == j) 0.0 else -sourceMatrix[i, j] / sourceMatrix[i, i]
            }

        }
        return transformedMatrix to transformedConstVector
    }

    private fun findSeidelTransformationMatrix(): RealMatrix {
        val (optimalH, _) = alphaOptimalSystem
        val rightMatrix = zeroMatrix(systemDimension)
        val leftMatrix = zeroMatrix(systemDimension)
        for(i in 0 until systemDimension) {
            for(j in 0 until systemDimension) {
                if(i <= j) {
                    rightMatrix[i, j] = optimalH[i, j]
                } else {
                    leftMatrix[i, j] = optimalH[i, j]
                }
            }
        }
        val identity = MatrixUtils.createRealIdentityMatrix(systemDimension)
        return (identity - leftMatrix).computeInverse() * rightMatrix
    }

    private fun refineWithLyusternikMethod(spectralRadius: Double, solutions: RealMatrix): RealVector {
        return solutions.getLastRow(1) +
                1.0 / (1 - spectralRadius) * (solutions.getLastRow(0) - solutions.getLastRow(1))
    }
}