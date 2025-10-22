'use client';

import { useState } from 'react';
import { useConfig, useUpdateConfig } from '@/lib/hooks';
import { useTranslations } from '@/lib/useTranslations';

export function ConfigCard() {
  const t = useTranslations('config');
  const { data: config, isLoading, error } = useConfig();
  const updateConfig = useUpdateConfig();
  
  const [isEditing, setIsEditing] = useState(false);
  const [tempMax, setTempMax] = useState('');
  const [humidityMax, setHumidityMax] = useState('');

  const handleEdit = () => {
    if (config) {
      setTempMax(config.tempMax.toString());
      setHumidityMax(config.humidityMax.toString());
      setIsEditing(true);
    }
  };

  const handleCancel = () => {
    setIsEditing(false);
    setTempMax('');
    setHumidityMax('');
  };

  const handleSave = async () => {
    try {
      await updateConfig.mutateAsync({
        tempMax: parseFloat(tempMax),
        humidityMax: parseFloat(humidityMax),
      });
      setIsEditing(false);
    } catch {
      // Error will be handled by the mutation
    }
  };

  if (isLoading) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="animate-pulse">
          <div className="h-4 bg-gray-200 rounded w-3/4 mb-4"></div>
          <div className="h-4 bg-gray-200 rounded w-1/2"></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="text-red-600">{t('errorMessage')}</div>
      </div>
    );
  }

  if (!config) return null;

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-xl font-bold text-gray-900 flex items-center gap-2">
          <svg className="w-6 h-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
          </svg>
          {t('title')}
        </h2>
      </div>

      {!isEditing ? (
        <>
          <div className="space-y-4">
            <div className="flex items-center justify-between p-3 bg-orange-50 rounded-lg">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center">
                  <svg className="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-600">{t('tempLabel')}</p>
                  <p className="text-2xl font-bold text-gray-900">{config.tempMax}°C</p>
                </div>
              </div>
            </div>

            <div className="flex items-center justify-between p-3 bg-blue-50 rounded-lg">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                  <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 15a4 4 0 004 4h9a5 5 0 10-.1-9.999 5.002 5.002 0 10-9.78 2.096A4.001 4.001 0 003 15z" />
                  </svg>
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-600">{t('humidityLabel')}</p>
                  <p className="text-2xl font-bold text-gray-900">{config.humidityMax}%</p>
                </div>
              </div>
            </div>

            <div className="pt-2">
              <p className="text-xs text-gray-500">
                {t('lastUpdate')}: {new Date(config.updatedAt).toLocaleString('es-ES')}
              </p>
            </div>
          </div>

          <button
            onClick={handleEdit}
            className="mt-4 w-full bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700 transition-colors duration-200 font-medium"
          >
            {t('saveButton')}
          </button>
        </>
      ) : (
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t('tempLabel')}
            </label>
            <input
              type="number"
              step="0.1"
              value={tempMax}
              onChange={(e) => setTempMax(e.target.value)}
              className="w-full px-3 py-2 border-2 border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 text-gray-900 font-semibold text-lg placeholder-gray-400"
              placeholder="100"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {t('humidityLabel')}
            </label>
            <input
              type="number"
              step="0.1"
              value={humidityMax}
              onChange={(e) => setHumidityMax(e.target.value)}
              className="w-full px-3 py-2 border-2 border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 text-gray-900 font-semibold text-lg placeholder-gray-400"
              placeholder="70"
            />
          </div>

          <div className="flex gap-2 pt-2">
            <button
              onClick={handleSave}
              disabled={updateConfig.isPending}
              className="flex-1 bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 transition-colors duration-200 font-medium disabled:bg-green-400"
            >
              {updateConfig.isPending ? `${t('saveButton')}...` : t('saveButton')}
            </button>
            <button
              onClick={handleCancel}
              disabled={updateConfig.isPending}
              className="flex-1 bg-gray-300 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-400 transition-colors duration-200 font-medium disabled:bg-gray-200"
            >
              Cancelar
            </button>
          </div>

          {updateConfig.isError && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm">
              Error al actualizar la configuración
            </div>
          )}
        </div>
      )}
    </div>
  );
}
