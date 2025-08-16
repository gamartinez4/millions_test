import WithAuth from '@/hoc/withAuth'
import { useUserData } from '@/hooks/useUserData'
import { useDisplayProperties } from '@/utils/propertyFilters'
import { useProperties } from '@/hooks/useProperties'
import { useImages } from '@/hooks/useImages'
import { useFilters } from '@/hooks/useFilters'
import { useNavigation } from '@/hooks/useNavigation'
import { usePropertiesWithImages } from '@/hooks/usePropertiesWithImages'
import { useBuyProperty } from '@/hooks/useBuyProperty'
import { getFirstPropertyImage, isPropertyOwned } from '@/utils/propertyFilters'
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
	Alert,
	Snackbar,
	CircularProgress,
} from '@mui/material'
import { Logout as LogoutIcon, ShoppingCart as ShoppingCartIcon } from '@mui/icons-material'
import { useEffect, useState } from 'react'
import { useAuth } from '@/context/AuthContext'


const DashboardPage = () => {


	const { user, loading: userLoading } = useUserData()

	const { images, loading: imagesLoading, error: imagesError } = useImages()

	
	const { properties: allProperties, loading: propertiesLoading, error: propertiesError } = useProperties()

	// Combinar propiedades con sus imágenes usando hook personalizado
	const propertiesWithImages = usePropertiesWithImages(allProperties, images)

	const loading = userLoading || propertiesLoading || imagesLoading

	if (loading) {
		return <Typography>Loading...</Typography>
	}

	if (propertiesError) {
		return <Typography color="error">Error: {propertiesError}</Typography>
	}

	if (imagesError) {
		return <Typography color="error">Error loading images: {imagesError}</Typography>
	}

	return <DashboardContent user={user} allProperties={propertiesWithImages} imagesLoading={imagesLoading} />
}

export default WithAuth(DashboardPage)





const DashboardContent = ({ user, allProperties, imagesLoading }) => {
	const { setSharedProperties } = useAuth()
	const [snackbarOpen, setSnackbarOpen] = useState(false)
	const [snackbarMessage, setSnackbarMessage] = useState('')
	const [snackbarSeverity, setSnackbarSeverity] = useState('success')

	const displayProperties = useDisplayProperties(allProperties, user)
	
	const {
		filters,
		filteredProperties,
		handleFilterChange,
		clearFilters,
		hasActiveFilters
	} = useFilters(displayProperties)

	const { handleViewMyProperties, handleLogout, handleViewPropertyDetails } = useNavigation()
	const { buyProperty, loading: buyLoading, error: buyError, clearError } = useBuyProperty()

	const navigateToMyProperties = () => {
		setSharedProperties(allProperties)
		handleViewMyProperties()
	}

	/**
	 * Handle property purchase
	 * @param {object} property - Property to buy
	 */
	const handleBuyProperty = async (property) => {
		try {
			console.log('handleBuyProperty called with:', { property, user })
			
			if (!user?.id) {
				console.error('User ID not found:', user)
				showSnackbar('Error: Usuario no identificado', 'error')
				return
			}

			if (!property?.id) {
				console.error('Property ID not found:', property)
				showSnackbar('Error: Propiedad no válida', 'error')
				return
			}

			console.log('Attempting to buy property:', { propertyId: property.id, userId: user.id })

			const success = await buyProperty(property.id, user.id)
			
			if (success) {
				showSnackbar('¡Propiedad comprada exitosamente!', 'success')
				// Optionally refresh the page or update the properties list
				window.location.reload()
			} else {
				showSnackbar('Error al comprar la propiedad', 'error')
			}
		} catch (error) {
			console.error('Buy property error:', error)
			showSnackbar('Error al comprar la propiedad', 'error')
		}
	}

	/**
	 * Show snackbar with message
	 * @param {string} message - Message to show
	 * @param {string} severity - Severity level (success, error, warning, info)
	 */
	const showSnackbar = (message, severity = 'success') => {
		setSnackbarMessage(message)
		setSnackbarSeverity(severity)
		setSnackbarOpen(true)
	}

	/**
	 * Close snackbar
	 */
	const handleCloseSnackbar = () => {
		setSnackbarOpen(false)
		clearError()
	}

	// Show error in snackbar if buy error occurs
	useEffect(() => {
		if (buyError) {
			showSnackbar(buyError, 'error')
		}
	}, [buyError])

	// Debug user data (can be removed in production)
	useEffect(() => {
		if (user) {
			console.log('Dashboard user data:', user)
		}
	}, [user])

	return (
		<Box sx={{ minHeight: '100vh', bgcolor: 'grey.50' }}>
			{/* Header */}
			<Paper elevation={1} sx={{ p: 2, mb: 3 }}>
				<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
					<Typography variant="h4">
						Hola, {user?.name}
					</Typography>
					<Box sx={{ display: 'flex', gap: 2 }}>
						<Button
							variant="contained"
							color="primary"
							onClick={navigateToMyProperties}
						>
							Ir a mis propiedades
						</Button>
						<Button
							variant="contained"
							color="secondary"
							onClick={handleLogout}
							startIcon={<LogoutIcon />}
						>
							Logout
						</Button>
					</Box>
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
								label="Nombre"
								value={filters.nameFilter}
								onChange={(e) => handleFilterChange('nameFilter', e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<TextField
								fullWidth
								label="Dirección"
								value={filters.addressFilter}
								onChange={(e) => handleFilterChange('addressFilter', e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<TextField
								fullWidth
								label="Precio mínimo"
								type="number"
								value={filters.minPriceFilter}
								onChange={(e) => handleFilterChange('minPriceFilter', e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<TextField
								fullWidth
								label="Precio máximo"
								type="number"
								value={filters.maxPriceFilter}
								onChange={(e) => handleFilterChange('maxPriceFilter', e.target.value)}
								margin="normal"
								size="small"
							/>
							
							<Button
								fullWidth
								variant="outlined"
								onClick={clearFilters}
								sx={{ mt: 2 }}
								disabled={!hasActiveFilters}
							>
								Limpiar filtros
							</Button>
							

						</Paper>
					</Box>

					{/* Right Side - Properties List (75% width) */}
					<Box sx={{ width: { xs: '100%', lg: '75%' } }}>
						<Typography variant="h5" gutterBottom>
							Propiedades ({filteredProperties.length})
						</Typography>
						
						<Grid container spacing={2}>
							{filteredProperties.map((property) => {
								const isOwned = isPropertyOwned(property, user)
								const firstImage = getFirstPropertyImage(property)
								
								// Debug property info (can be removed in production)
								if (property.forSale && !isOwned) {
									console.log('Property available for purchase:', {
										id: property.id,
										name: property.name,
										forSale: property.forSale,
										isOwned,
										userId: user?.id
									})
								}
								
								return (
									<Grid xs={12} sm={6} lg={4} key={property.id}>
										<Card 
											elevation={2} 
											sx={{ 
												height: '100%',
												cursor: 'pointer',
												transition: 'transform 0.2s, elevation 0.2s',
												'&:hover': {
													transform: 'translateY(-4px)',
													elevation: 4
												}
											}}
											onClick={() => handleViewPropertyDetails(property)}
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
													{isOwned && (
														<Box sx={{ 
															bgcolor: 'primary.main', 
															color: 'white', 
															px: 1, 
															py: 0.5, 
															borderRadius: 1,
															fontSize: '0.75rem'
														}}>
															Mía
														</Box>
													)}
												</Box>
												
												<Typography variant="body2" color="text.secondary" gutterBottom>
													{property.address}
												</Typography>
												
												<Typography variant="h6" color="primary">
													${property.price.toLocaleString()}
												</Typography>
												
												<Typography variant="body2" color="text.secondary">
													Año: {property.year}
												</Typography>
												
												{property.ownerName && (
													<Typography variant="body2" color="text.secondary">
														Propietario: {property.ownerName}
													</Typography>
												)}
												
												{property.forSale && !isOwned && (
													<Box sx={{ mt: 1 }}>
														<Box sx={{ 
															bgcolor: 'success.main', 
															color: 'white', 
															p: 1, 
															borderRadius: 1,
															textAlign: 'center',
															mb: 1
														}}>
															<Typography variant="body2">
																¡Disponible para comprar!
															</Typography>
														</Box>
														<Button
															fullWidth
															variant="contained"
															color="primary"
															startIcon={buyLoading ? <CircularProgress size={20} color="inherit" /> : <ShoppingCartIcon />}
															onClick={(e) => {
																e.stopPropagation()
																handleBuyProperty(property)
															}}
															disabled={buyLoading}
															sx={{
																bgcolor: 'success.main',
																'&:hover': {
																	bgcolor: 'success.dark'
																}
															}}
														>
															{buyLoading ? 'Comprando...' : 'Comprar'}
														</Button>
													</Box>
												)}
											</CardContent>
										</Card>
									</Grid>
								)
							})}
						</Grid>
						
						{filteredProperties.length === 0 && displayProperties.length > 0 && (
							<Paper sx={{ p: 4, textAlign: 'center', mt: 2 }}>
								<Typography variant="h6" color="text.secondary">
									{hasActiveFilters 
										? 'No se encontraron propiedades con los filtros aplicados'
										: 'No hay propiedades disponibles para mostrar'
									}
								</Typography>
								{hasActiveFilters && (
									<Button 
										variant="outlined" 
										onClick={clearFilters}
										sx={{ mt: 2 }}
									>
										Limpiar filtros
									</Button>
								)}
							</Paper>
						)}

						{displayProperties.length === 0 && !imagesLoading && (
							<Paper sx={{ p: 4, textAlign: 'center', mt: 2 }}>
								<Typography variant="h6" color="text.secondary">
									No hay propiedades disponibles
								</Typography>
							</Paper>
						)}
					</Box>
				</Box>
			</Container>

			{/* Snackbar for notifications */}
			<Snackbar
				open={snackbarOpen}
				autoHideDuration={6000}
				onClose={handleCloseSnackbar}
				anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
			>
				<Alert 
					onClose={handleCloseSnackbar} 
					severity={snackbarSeverity}
					sx={{ width: '100%' }}
				>
					{snackbarMessage}
				</Alert>
			</Snackbar>
		</Box>
	)
}









