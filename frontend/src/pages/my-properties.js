import WithAuth from '@/hoc/withAuth'
import { useAuth } from '@/context/AuthContext'
import { useRouter } from 'next/router'
import { useState, useEffect } from 'react'
import axios from 'axios'
import {
	Container,
	Typography,
	Box,
	Grid,
	Card,
	CardContent,
	Paper,
	TextField,
	Button,
	CardMedia,
	IconButton,
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	FormControl,
	InputLabel,
	Select,
	MenuItem,
	CircularProgress,
	Alert,
} from '@mui/material'
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material'
import { useImages } from '@/hooks/useImages'
import { useProperties } from '@/hooks/useProperties'
import { usePropertiesWithImages } from '@/hooks/usePropertiesWithImages'
import { useUserProperties } from '@/hooks/useUserProperties'
import { usePropertyTextFilters } from '@/hooks/usePropertyTextFilters'

const MyPropertiesPage = () => {
	const { user } = useAuth()
	const { images, loading: imagesLoading, error: imagesError } = useImages()
	const { properties: allProperties, loading: propertiesLoading, error: propertiesError } = useProperties()
	const propertiesWithImages = usePropertiesWithImages(allProperties, images)

	const loading = propertiesLoading || imagesLoading

	if (loading) {
		return (
			<Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
				<Typography>Cargando tus propiedades...</Typography>
			</Box>
		)
	}

	if (propertiesError || imagesError) {
		return (
			<Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
				<Typography color="error">
					Error al cargar los datos: {propertiesError || imagesError}
				</Typography>
			</Box>
		)
	}

	return <MyPropertiesContent user={user} propertiesWithImages={propertiesWithImages} />
}

export default WithAuth(MyPropertiesPage)


const MyPropertiesContent = ({ user, propertiesWithImages }) => {
	const router = useRouter()
	const { setSharedProperties } = useAuth()

	// --- Local Properties State ---
	const [localProperties, setLocalProperties] = useState(propertiesWithImages)

	// Sync local properties when propertiesWithImages changes
	useEffect(() => {
		setLocalProperties(propertiesWithImages)
	}, [propertiesWithImages])

	// --- Custom Hooks for Logic ---
	const userProperties = useUserProperties(localProperties, user)
	const {
		nameFilter,
		addressFilter,
		setNameFilter,
		setAddressFilter,
		filteredProperties,
		clearFilters,
	} = usePropertyTextFilters(userProperties)
	// ---------------------------

	// --- Modal State ---
	const [editModalOpen, setEditModalOpen] = useState(false)
	const [selectedProperty, setSelectedProperty] = useState(null)
	const [editPrice, setEditPrice] = useState('')
	const [editForSale, setEditForSale] = useState(false)
	const [isLoading, setIsLoading] = useState(false)
	const [error, setError] = useState(null)
	const [success, setSuccess] = useState(false)
	// ----------------

	const handleBackToDashboard = () => {
		router.push('/dashboard')
	}

	// --- Modal Functions ---
	const handleOpenEditModal = (property) => {
		setSelectedProperty(property)
		setEditPrice(property.price.toString())
		setEditForSale(property.forSale)
		setEditModalOpen(true)
		setError(null)
		setSuccess(false)
	}

	const handleCloseEditModal = () => {
		if (isLoading) return // Prevent closing while loading
		setEditModalOpen(false)
		setSelectedProperty(null)
		setEditPrice('')
		setEditForSale(false)
		setError(null)
		setSuccess(false)
	}

	const handleSaveChanges = async () => {
		if (!selectedProperty) return

		const newPrice = parseFloat(editPrice)
		if (isNaN(newPrice) || newPrice < 0) {
			setError('Por favor ingresa un precio válido.')
			return
		}

		setIsLoading(true)
		setError(null)

		try {
			const calls = []

			// Si el precio cambió, llamar a la API de precio
			if (newPrice !== selectedProperty.price) {
				calls.push(
					axios.patch(`/api/properties/update-price?propertyId=${selectedProperty.id}`, {
						price: newPrice
					})
				)
			}

			// Si el estado forSale cambió, llamar a la API de forSale
			if (editForSale !== selectedProperty.forSale) {
				calls.push(
					axios.patch(`/api/properties/update-for-sale?propertyId=${selectedProperty.id}`, {
						forSale: editForSale
					})
				)
			}

			// Si hay cambios, ejecutar las llamadas
			if (calls.length > 0) {
				await Promise.all(calls)
			}

			// Actualizar la propiedad en el estado local
			const updatedProperty = {
				...selectedProperty,
				price: newPrice,
				forSale: editForSale
			}

			// Actualizar el estado local de las propiedades
			setLocalProperties(prevProperties => 
				prevProperties.map(prop => 
					prop.id === selectedProperty.id ? updatedProperty : prop
				)
			)

			// También actualizar el contexto compartido si existe
			if (setSharedProperties) {
				setSharedProperties(prevProperties => 
					prevProperties ? prevProperties.map(prop => 
						prop.id === selectedProperty.id ? updatedProperty : prop
					) : null
				)
			}

			// Mostrar éxito
			setSuccess(true)

			// Cerrar el modal después de un breve delay
			setTimeout(() => {
				handleCloseEditModal()
			}, 1500)

		} catch (error) {
			console.error('Error al actualizar la propiedad:', error)
			
			let errorMessage = 'Error al actualizar la propiedad. Inténtalo de nuevo.'
			
			if (error.response?.status === 404) {
				errorMessage = 'La propiedad no fue encontrada.'
			} else if (error.response?.status === 401) {
				errorMessage = 'No tienes autorización para realizar esta acción.'
			} else if (error.response?.data?.message) {
				errorMessage = error.response.data.message
			}

			setError(errorMessage)
		} finally {
			setIsLoading(false)
		}
	}
	// --------------------

	return (
		<Box sx={{ minHeight: '100vh', bgcolor: 'grey.50' }}>
			{/* Header */}
			<Paper elevation={1} sx={{ p: 2, mb: 3 }}>
				<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
					<Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
						<IconButton onClick={handleBackToDashboard} color="primary">
							<ArrowBackIcon />
						</IconButton>
						<Typography variant="h4">
							Mis Propiedades - {user?.name}
						</Typography>
					</Box>
					<Button
						variant="outlined"
						onClick={handleBackToDashboard}
					>
						Volver al Dashboard
					</Button>
				</Box>
			</Paper>

			<Container maxWidth="xl">
				<Box sx={{ 
					display: 'flex', 
					gap: 3,
					flexDirection: { xs: 'column', lg: 'row' }
				}}>
					{/* Left Sidebar - Filters (25% width) */}
					<Box sx={{ 
						width: { xs: '100%', lg: '25%' }, 
						minWidth: { xs: 'auto', lg: '300px' }
					}}>
						<Paper elevation={2} sx={{ p: 3, position: 'sticky', top: 16 }}>
							<Typography variant="h6" gutterBottom>
								Filtros
							</Typography>
							
							<TextField
								fullWidth
								label="Nombre de la propiedad"
								value={nameFilter}
								onChange={(e) => setNameFilter(e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<TextField
								fullWidth
								label="Dirección"
								value={addressFilter}
								onChange={(e) => setAddressFilter(e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<Button
								fullWidth
								variant="outlined"
								onClick={clearFilters}
								sx={{ mt: 2 }}
							>
								Limpiar filtros
							</Button>
							
							<Box sx={{ mt: 3, p: 2, bgcolor: 'grey.100', borderRadius: 1 }}>
								<Typography variant="body2" color="text.secondary">
									<strong>Propiedades totales:</strong> {userProperties.length}
								</Typography>
								<Typography variant="body2" color="text.secondary">
									<strong>Filtradas:</strong> {filteredProperties.length}
								</Typography>
							</Box>
						</Paper>
					</Box>

					{/* Right Side - Properties List (75% width) */}
					<Box sx={{ width: { xs: '100%', lg: '75%' } }}>
						<Typography variant="h5" gutterBottom>
							Mis Propiedades ({filteredProperties.length})
						</Typography>
						
						<Grid container spacing={2}>
							{filteredProperties.map((property) => {
								const firstImage = property.images && property.images.length > 0 ? property.images[0] : null
								
								return (
									<Grid item xs={12} sm={6} lg={4} key={property.id}>
										<Card 
											elevation={2} 
											sx={{ 
												height: '100%',
												cursor: 'pointer',
												transition: 'transform 0.2s, elevation 0.2s',
												'&:hover': {
													transform: 'translateY(-2px)',
													elevation: 4
												}
											}}
											onClick={() => handleOpenEditModal(property)}
										>
											<CardMedia
												sx={{ 
													height: 200, 
													bgcolor: 'grey.200',
													display: 'flex',
													alignItems: 'center',
													justifyContent: 'center'
												}}
												image={firstImage?.fileUrl || undefined}
											>
												{!firstImage && (
													<Typography variant="body2" color="text.secondary">
														[Imagen no disponible]
													</Typography>
												)}
											</CardMedia>
											
											<CardContent>
												<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
													<Typography variant="h6" component="h3">
														{property.name}
													</Typography>
													<Box sx={{ 
														bgcolor: 'primary.main', 
														color: 'white', 
														px: 1, 
														py: 0.5, 
														borderRadius: 1,
														fontSize: '0.75rem'
													}}>
														Mi Propiedad
													</Box>
												</Box>
												
												<Typography variant="body2" color="text.secondary" gutterBottom>
													{property.address}
												</Typography>
												
												{property.forSale && (
													<Typography variant="h6" color="primary">
														${property.price.toLocaleString()}
													</Typography>
												)}
												
												<Typography variant="body2" color="text.secondary">
													Año: {property.year}
												</Typography>
												
												{property.forSale && (
													<Box sx={{ 
														bgcolor: 'success.main', 
														color: 'white', 
														p: 1, 
														borderRadius: 1,
														mt: 1,
														textAlign: 'center'
													}}>
														<Typography variant="body2">
															¡En venta!
														</Typography>
													</Box>
												)}
											</CardContent>
										</Card>
									</Grid>
								)
							})}
						</Grid>
						
						{filteredProperties.length === 0 && userProperties.length > 0 && (
							<Paper sx={{ p: 4, textAlign: 'center', mt: 2 }}>
								<Typography variant="h6" color="text.secondary">
									No se encontraron propiedades con los filtros aplicados
								</Typography>
							</Paper>
						)}
						
						{userProperties.length === 0 && (
							<Paper sx={{ p: 4, textAlign: 'center', mt: 2 }}>
								<Typography variant="h6" color="text.secondary">
									No tienes propiedades registradas
								</Typography>
								<Typography variant="body1" color="text.secondary" sx={{ mt: 1 }}>
									Cuando tengas propiedades, aparecerán aquí
								</Typography>
							</Paper>
						)}
					</Box>
				</Box>
			</Container>

			{/* Modal de Edición */}
			<Dialog 
				open={editModalOpen} 
				onClose={handleCloseEditModal}
				maxWidth="sm"
				fullWidth
			>
				<DialogTitle>
					Editar Propiedad
				</DialogTitle>
				<DialogContent>
					{selectedProperty && (
						<Box sx={{ pt: 1 }}>
							<Typography variant="h6" gutterBottom>
								{selectedProperty.name}
							</Typography>
							<Typography variant="body2" color="text.secondary" gutterBottom>
								{selectedProperty.address}
							</Typography>

							{error && (
								<Alert severity="error" sx={{ mb: 2 }}>
									{error}
								</Alert>
							)}

							{success && (
								<Alert severity="success" sx={{ mb: 2 }}>
									¡Propiedad actualizada exitosamente!
								</Alert>
							)}

							<TextField
								fullWidth
								label="Precio"
								type="number"
								value={editPrice}
								onChange={(e) => setEditPrice(e.target.value)}
								margin="normal"
								disabled={isLoading}
								inputProps={{ min: 0, step: 0.01 }}
								InputProps={{
									startAdornment: <Typography sx={{ mr: 1 }}>$</Typography>
								}}
							/>

							<FormControl fullWidth margin="normal" disabled={isLoading}>
								<InputLabel>Estado de Venta</InputLabel>
								<Select
									value={editForSale}
									label="Estado de Venta"
									onChange={(e) => setEditForSale(e.target.value)}
								>
									<MenuItem value={true}>En Venta</MenuItem>
									<MenuItem value={false}>No en Venta</MenuItem>
								</Select>
							</FormControl>
						</Box>
					)}
				</DialogContent>
				<DialogActions>
					<Button 
						onClick={handleCloseEditModal}
						disabled={isLoading}
					>
						Cancelar
					</Button>
					<Button 
						onClick={handleSaveChanges}
						variant="contained"
						disabled={isLoading}
						startIcon={isLoading ? <CircularProgress size={20} /> : null}
					>
						{isLoading ? 'Guardando...' : 'Guardar'}
					</Button>
				</DialogActions>
			</Dialog>
		</Box>
	)
}