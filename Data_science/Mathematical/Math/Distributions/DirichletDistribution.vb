﻿#Region "Microsoft.VisualBasic::298255a98178c51bb61d30a0efe186b3, ..\sciBASIC#\Data_science\Mathematical\Math\Distributions\DirichletDistribution.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Namespace Distributions

    ''' <summary>
    ''' Dirichlet distribution
    ''' 
    ''' In probability and statistics, the Dirichlet distribution (after Peter Gustav Lejeune Dirichlet), often denoted 
    ''' {\displaystyle \operatorname {Dir} ({\boldsymbol {\alpha }})} \operatorname {Dir} ({\boldsymbol {\alpha }}), is 
    ''' a family of continuous multivariate probability distributions parameterized by a vector 
    ''' {\displaystyle {\boldsymbol {\alpha }}} {\boldsymbol {\alpha }} of positive reals. 
    ''' It is a multivariate generalization of the beta distribution.[1] Dirichlet distributions are very often used as 
    ''' prior distributions in Bayesian statistics, and in fact the Dirichlet distribution is the conjugate prior of the 
    ''' categorical distribution and multinomial distribution.
    ''' </summary>
    Public Class DirichletDistribution

        Public a1 As Double = 1.0F
        Public a2 As Double = 1.0F
        Public a3 As Double = 1.0F

        Public Function Probability(x1 As Double, x2 As Double, x3 As Double) As Double
            Dim logCoef As Double = lgamma(a1 + a2 + a3) - lgamma(a1) - lgamma(a2) - lgamma(a3)
            Dim logValue As Double =
                (a1 - 1.0F) * Math.Log(x1) +
                (a2 - 1.0F) * Math.Log(x2) +
                (a3 - 1.0F) * Math.Log(x3)

            Return Math.Exp(logCoef + logValue)
        End Function

        ''' <summary>
        ''' see http://www.machinedlearnings.com/2011/06/faster-lda.html
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>Works fun with function <see cref="Beta.beta(Double, Double, Double)"/></remarks>
        Public Shared Function lgamma(x As Double) As Double
            Dim logterm As Double = Math.Log(x * (1.0F + x) * (2.0F + x))
            Dim xp3 As Double = 3.0F + x

            Return -2.081061F - x + 0.0833333F / xp3 - logterm + (2.5F + x) * Math.Log(xp3)
        End Function
    End Class
End Namespace
