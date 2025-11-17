import { useEffect, useState } from "react";
import type { Incident } from "../api/incidentService";
import { IncidentService } from "../api/incidentService";
import { useNavigate, useParams, Link } from "react-router-dom";

export default function IncidentEdit() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [incident, setIncident] = useState<Incident | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    IncidentService.getById(Number(id))
      .then((res: { data: Incident }) => setIncident(res.data))
      .catch(() => setError("Failed to load incident"))
      .finally(() => setLoading(false));
  }, [id]);

  const save = async () => {
    if (!incident) return;

    setSaving(true);
    setError("");

    try {
      await IncidentService.update(incident.id, {
        description: incident.description,
        resolution: incident.resolution,
        status: incident.status,
      });
      navigate("/incidents");
    } catch (err) {
      setError("Failed to update incident. Please try again.");
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
          <p className="text-gray-600 text-lg">Loading incident...</p>
        </div>
      </div>
    );
  }

  if (!incident) {
    return (
      <div className="max-w-2xl mx-auto">
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
          <div className="text-6xl mb-4">❌</div>
          <h3 className="text-xl font-semibold text-gray-900 mb-2">
            Incident not found
          </h3>
          <p className="text-gray-600 mb-6">
            The incident you're looking for doesn't exist or has been deleted.
          </p>
          <Link
            to="/incidents"
            className="inline-flex items-center gap-2 bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium"
          >
            ← Back to Incidents
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto">
      {/* Header */}
      <div className="mb-6">
        <Link
          to="/incidents"
          className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <span>←</span> Back to Incidents
        </Link>
        <div className="flex items-center gap-3 mb-2">
          <h1 className="text-3xl font-bold text-gray-900">Edit Incident</h1>
          <span className="px-3 py-1 bg-gray-100 text-gray-700 rounded-lg font-mono text-sm">
            #{incident.id}
          </span>
        </div>
        <p className="text-gray-600">{incident.title}</p>
      </div>

      {/* Form Card */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
        <form
          onSubmit={(e) => {
            e.preventDefault();
            save();
          }}
          className="space-y-6"
        >
          {/* Title Display (Read-only) */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Title
            </label>
            <div className="px-4 py-3 bg-gray-50 border border-gray-200 rounded-lg text-gray-700">
              {incident.title}
            </div>
          </div>

          {/* Description Field */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Description
            </label>
            <textarea
              rows={6}
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all resize-none"
              value={incident.description}
              onChange={(e) =>
                setIncident({ ...incident, description: e.target.value })
              }
              placeholder="Describe the incident..."
              disabled={saving}
            />
          </div>

          {/* Status Field */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Status
            </label>
            <select
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all bg-white"
              value={incident.status}
              onChange={(e) =>
                setIncident({
                  ...incident,
                  status: e.target.value as "Open" | "InProgress" | "Closed",
                })
              }
              disabled={saving}
            >
              <option value="Open">🔴 Open</option>
              <option value="InProgress">🟡 In Progress</option>
              <option value="Closed">🟢 Closed</option>
            </select>
          </div>

          {/* Resolution Field (shown when status is Closed) */}
          {incident.status === "Closed" && (
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Resolution <span className="text-gray-500 font-normal">(required for closed incidents)</span>
              </label>
              <textarea
                rows={4}
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all resize-none"
                value={incident.resolution || ""}
                onChange={(e) =>
                  setIncident({ ...incident, resolution: e.target.value })
                }
                placeholder="Describe how this incident was resolved..."
                disabled={saving}
              />
            </div>
          )}

          {/* Error Message */}
          {error && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4">
              <p className="text-sm text-red-800">{error}</p>
            </div>
          )}

          {/* Action Buttons */}
          <div className="flex items-center gap-4 pt-4 border-t border-gray-200">
            <button
              type="submit"
              disabled={saving}
              className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-600 to-blue-700 text-white px-6 py-3 rounded-lg shadow-lg hover:from-blue-700 hover:to-blue-800 transition-all duration-200 font-medium disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 transform hover:scale-105"
            >
              {saving ? (
                <>
                  <div className="inline-block animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  Saving...
                </>
              ) : (
                <>
                  <span>💾</span>
                  Save Changes
                </>
              )}
            </button>
            <Link
              to="/incidents"
              className="px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
            >
              Cancel
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}
