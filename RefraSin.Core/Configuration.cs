using System;
using MathNet.Numerics.Random;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RefraSin.Core
{
    /// <summary>
    /// Static configuration class for this library.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Configures this library to use an <see cref="IHost"/> as source for loggers. Annihilates the effect of <see cref="UseLoggerFactory"/>. 
        /// </summary>
        /// <param name="host">an <see cref="IHost"/> instance with support for <see cref="ILogger{TCategoryName}"/> dependency injection</param>
        public static void UseHost(IHost host)
        {
            _host = host;
            _loggerFactory = null;
        }

        /// <summary>
        /// Configures this library to use an <see cref="ILoggerFactory"/> as source for loggers. Annihilates the effect of <see cref="UseHost"/>.
        /// </summary>
        /// <param name="loggerFactory"></param>
        public static void UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _host = null;
            _loggerFactory = loggerFactory;
        }

        private static ILoggerFactory? _loggerFactory;
        private static IHost? _host;

        internal static ILogger<T> CreateLogger<T>() => _host?.Services.GetRequiredService<ILogger<T>>() ?? _loggerFactory?.CreateLogger<T>() ?? NullLogger<T>.Instance;
        
        internal static ILogger CreateLogger(Type type) => (_loggerFactory ?? _host?.Services.GetRequiredService<ILoggerFactory>())?.CreateLogger(type) ?? NullLogger.Instance;
        
        /// <summary>
        ///     Event raised when <see cref="RandomSource" /> property is changed to notify referrers.
        /// </summary>
        public static event EventHandler? RandomSourceChanged;
        
        private static RandomSource _randomSource = MersenneTwister.Default;

        /// <summary>
        ///     Random source to use in all sampling.
        /// </summary>
        public static RandomSource RandomSource
        {
            get => _randomSource;
            set
            {
                _randomSource = value;
                RandomSourceChanged?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}